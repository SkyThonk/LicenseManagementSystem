/**
 * License Management System - API Service
 * Handles all API communications with the backend services
 */

(function () {
    'use strict';

    // ============================================
    // API Configuration
    // ============================================
    const ApiConfig = {
        baseUrl: '',
        timeout: 30000,
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    };

    // ============================================
    // API Service Class
    // ============================================
    class ApiService {
        constructor(config = {}) {
            this.baseUrl = config.baseUrl || ApiConfig.baseUrl;
            this.timeout = config.timeout || ApiConfig.timeout;
            this.defaultHeaders = { ...ApiConfig.headers, ...config.headers };
            this.requestInterceptors = [];
            this.responseInterceptors = [];
        }

        // Add request interceptor
        addRequestInterceptor(interceptor) {
            this.requestInterceptors.push(interceptor);
            return this;
        }

        // Add response interceptor
        addResponseInterceptor(interceptor) {
            this.responseInterceptors.push(interceptor);
            return this;
        }

        // Build URL with query parameters
        buildUrl(endpoint, params = {}) {
            const url = new URL(endpoint, window.location.origin);
            Object.keys(params).forEach(key => {
                if (params[key] !== undefined && params[key] !== null) {
                    url.searchParams.append(key, params[key]);
                }
            });
            return url.toString();
        }

        // Execute request
        async request(method, endpoint, options = {}) {
            let config = {
                method,
                headers: { ...this.defaultHeaders, ...options.headers },
                credentials: 'same-origin'
            };

            // Apply request interceptors
            for (const interceptor of this.requestInterceptors) {
                config = await interceptor(config);
            }

            // Add body for non-GET requests
            if (options.body && method !== 'GET') {
                if (options.body instanceof FormData) {
                    delete config.headers['Content-Type'];
                    config.body = options.body;
                } else {
                    config.body = JSON.stringify(options.body);
                }
            }

            // Build URL with query params
            const url = this.buildUrl(
                this.baseUrl + endpoint,
                options.params
            );

            try {
                // Show loading state
                if (options.showLoading !== false) {
                    this.showLoading();
                }

                const controller = new AbortController();
                const timeoutId = setTimeout(() => controller.abort(), this.timeout);
                config.signal = controller.signal;

                const response = await fetch(url, config);
                clearTimeout(timeoutId);

                // Apply response interceptors
                let result = response;
                for (const interceptor of this.responseInterceptors) {
                    result = await interceptor(result);
                }

                // Handle response
                if (!response.ok) {
                    const error = await this.parseError(response);
                    throw error;
                }

                // Parse response
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('application/json')) {
                    return await response.json();
                }
                
                return await response.text();

            } catch (error) {
                if (error.name === 'AbortError') {
                    throw new Error('Request timeout');
                }
                throw error;
            } finally {
                if (options.showLoading !== false) {
                    this.hideLoading();
                }
            }
        }

        // Parse error response
        async parseError(response) {
            let errorData;
            try {
                errorData = await response.json();
            } catch {
                errorData = { message: response.statusText };
            }

            const error = new Error(errorData.message || errorData.title || 'An error occurred');
            error.status = response.status;
            error.data = errorData;
            return error;
        }

        // HTTP methods
        get(endpoint, params = {}, options = {}) {
            return this.request('GET', endpoint, { ...options, params });
        }

        post(endpoint, body = {}, options = {}) {
            return this.request('POST', endpoint, { ...options, body });
        }

        put(endpoint, body = {}, options = {}) {
            return this.request('PUT', endpoint, { ...options, body });
        }

        patch(endpoint, body = {}, options = {}) {
            return this.request('PATCH', endpoint, { ...options, body });
        }

        delete(endpoint, options = {}) {
            return this.request('DELETE', endpoint, options);
        }

        // Loading indicator
        showLoading() {
            let loader = document.getElementById('api-loader');
            if (!loader) {
                loader = document.createElement('div');
                loader.id = 'api-loader';
                loader.className = 'api-loader';
                loader.innerHTML = '<div class="api-loader-spinner"></div>';
                document.body.appendChild(loader);
            }
            loader.classList.add('show');
        }

        hideLoading() {
            const loader = document.getElementById('api-loader');
            if (loader) {
                loader.classList.remove('show');
            }
        }
    }

    // ============================================
    // Create API instance
    // ============================================
    const api = new ApiService();

    // Add default response interceptor for error handling
    api.addResponseInterceptor(async (response) => {
        if (response.status === 401) {
            window.location.href = '/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
        }
        return response;
    });

    // ============================================
    // Tenant API
    // ============================================
    const TenantApi = {
        getAll: (params = {}) => api.get('/api/tenants', params),
        getById: (id) => api.get(`/api/tenants/${id}`),
        create: (data) => api.post('/api/tenants', data),
        update: (id, data) => api.put(`/api/tenants/${id}`, data),
        delete: (id) => api.delete(`/api/tenants/${id}`),
        activate: (id) => api.post(`/api/tenants/${id}/activate`),
        deactivate: (id) => api.post(`/api/tenants/${id}/deactivate`)
    };

    // ============================================
    // License API
    // ============================================
    const LicenseApi = {
        getAll: (params = {}) => api.get('/api/licenses', params),
        getById: (id) => api.get(`/api/licenses/${id}`),
        create: (data) => api.post('/api/licenses', data),
        update: (id, data) => api.put(`/api/licenses/${id}`, data),
        delete: (id) => api.delete(`/api/licenses/${id}`),
        activate: (id) => api.post(`/api/licenses/${id}/activate`),
        deactivate: (id) => api.post(`/api/licenses/${id}/deactivate`),
        renew: (id, data) => api.post(`/api/licenses/${id}/renew`, data),
        validate: (key) => api.post('/api/licenses/validate', { key })
    };

    // ============================================
    // Payment API
    // ============================================
    const PaymentApi = {
        getAll: (params = {}) => api.get('/api/payments', params),
        getById: (id) => api.get(`/api/payments/${id}`),
        create: (data) => api.post('/api/payments', data),
        refund: (id) => api.post(`/api/payments/${id}/refund`),
        getByLicense: (licenseId) => api.get(`/api/payments/license/${licenseId}`)
    };

    // ============================================
    // Document API
    // ============================================
    const DocumentApi = {
        getAll: (params = {}) => api.get('/api/documents', params),
        getById: (id) => api.get(`/api/documents/${id}`),
        upload: (formData) => api.post('/api/documents', formData, {
            headers: {} // Let browser set Content-Type for FormData
        }),
        delete: (id) => api.delete(`/api/documents/${id}`),
        getDownloadUrl: (id) => api.get(`/api/documents/${id}/download-url`)
    };

    // ============================================
    // Notification API
    // ============================================
    const NotificationApi = {
        getAll: (params = {}) => api.get('/api/notifications', params),
        getUnread: () => api.get('/api/notifications/unread'),
        markAsRead: (id) => api.post(`/api/notifications/${id}/read`),
        markAllAsRead: () => api.post('/api/notifications/read-all'),
        delete: (id) => api.delete(`/api/notifications/${id}`)
    };

    // ============================================
    // Dashboard API
    // ============================================
    const DashboardApi = {
        getStats: () => api.get('/api/dashboard/stats'),
        getRecentActivity: () => api.get('/api/dashboard/activity'),
        getChartData: (type, params = {}) => api.get(`/api/dashboard/charts/${type}`, params)
    };

    // ============================================
    // Export to window
    // ============================================
    window.Api = {
        service: api,
        Tenant: TenantApi,
        License: LicenseApi,
        Payment: PaymentApi,
        Document: DocumentApi,
        Notification: NotificationApi,
        Dashboard: DashboardApi
    };

})();
