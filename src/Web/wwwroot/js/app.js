/**
 * License Management System - Main JavaScript
 * Aurora-inspired modern admin template
 */

(function () {
    'use strict';

    // ============================================
    // Global Configuration
    // ============================================
    const Config = {
        sidebarCollapsedKey: 'sidebar-collapsed',
        themeKey: 'theme-mode',
        animationDuration: 300
    };

    // ============================================
    // DOM Ready
    // ============================================
    document.addEventListener('DOMContentLoaded', function () {
        initSidebar();
        initDropdowns();
        initModals();
        initAlerts();
        initTooltips();
        initTables();
        initForms();
        initTheme();
        initSearch();
    });

    // ============================================
    // Sidebar Functions
    // ============================================
    function initSidebar() {
        const sidebar = document.querySelector('.sidebar');
        const sidebarToggle = document.querySelector('.sidebar-toggle');
        const mobileMenuToggle = document.querySelector('.mobile-menu-toggle');
        const sidebarOverlay = document.querySelector('.sidebar-overlay');

        if (!sidebar) return;

        // Restore sidebar state from localStorage
        const isCollapsed = localStorage.getItem(Config.sidebarCollapsedKey) === 'true';
        if (isCollapsed && window.innerWidth > 1024) {
            sidebar.classList.add('collapsed');
        }

        // Desktop toggle
        if (sidebarToggle) {
            sidebarToggle.addEventListener('click', function () {
                sidebar.classList.toggle('collapsed');
                localStorage.setItem(Config.sidebarCollapsedKey, sidebar.classList.contains('collapsed'));
            });
        }

        // Mobile toggle
        if (mobileMenuToggle) {
            mobileMenuToggle.addEventListener('click', function () {
                sidebar.classList.toggle('show');
                if (sidebarOverlay) {
                    sidebarOverlay.classList.toggle('show');
                }
            });
        }

        // Close sidebar on overlay click
        if (sidebarOverlay) {
            sidebarOverlay.addEventListener('click', function () {
                sidebar.classList.remove('show');
                sidebarOverlay.classList.remove('show');
            });
        }

        // Submenu toggle
        const submenuToggles = document.querySelectorAll('.nav-link[data-submenu]');
        submenuToggles.forEach(function (toggle) {
            toggle.addEventListener('click', function (e) {
                e.preventDefault();
                const navItem = this.closest('.nav-item');
                navItem.classList.toggle('open');
            });
        });

        // Handle window resize
        window.addEventListener('resize', function () {
            if (window.innerWidth > 1024) {
                sidebar.classList.remove('show');
                if (sidebarOverlay) {
                    sidebarOverlay.classList.remove('show');
                }
            }
        });
    }

    // ============================================
    // Dropdown Functions
    // ============================================
    function initDropdowns() {
        const dropdowns = document.querySelectorAll('[data-dropdown]');

        dropdowns.forEach(function (dropdown) {
            const trigger = dropdown.querySelector('[data-dropdown-trigger]');

            if (trigger) {
                trigger.addEventListener('click', function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    // Close other dropdowns
                    dropdowns.forEach(function (d) {
                        if (d !== dropdown) {
                            d.classList.remove('open');
                        }
                    });

                    dropdown.classList.toggle('open');
                });
            }
        });

        // Close dropdowns when clicking outside
        document.addEventListener('click', function (e) {
            if (!e.target.closest('[data-dropdown]')) {
                dropdowns.forEach(function (dropdown) {
                    dropdown.classList.remove('open');
                });
            }
        });

        // Close dropdowns on escape key
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                dropdowns.forEach(function (dropdown) {
                    dropdown.classList.remove('open');
                });
            }
        });
    }

    // ============================================
    // Modal Functions
    // ============================================
    function initModals() {
        // Open modal triggers
        document.querySelectorAll('[data-modal-open]').forEach(function (trigger) {
            trigger.addEventListener('click', function (e) {
                e.preventDefault();
                const modalId = this.getAttribute('data-modal-open');
                openModal(modalId);
            });
        });

        // Close modal triggers
        document.querySelectorAll('[data-modal-close]').forEach(function (trigger) {
            trigger.addEventListener('click', function (e) {
                e.preventDefault();
                const modal = this.closest('.modal');
                if (modal) {
                    closeModal(modal.id);
                }
            });
        });

        // Close modal on overlay click
        document.querySelectorAll('.modal-overlay').forEach(function (overlay) {
            overlay.addEventListener('click', function (e) {
                if (e.target === this) {
                    const modal = this.closest('.modal');
                    if (modal && !modal.hasAttribute('data-modal-static')) {
                        closeModal(modal.id);
                    }
                }
            });
        });

        // Close modal on escape key
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                const openModal = document.querySelector('.modal.show');
                if (openModal && !openModal.hasAttribute('data-modal-static')) {
                    closeModal(openModal.id);
                }
            }
        });
    }

    window.openModal = function (modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.classList.add('show');
            document.body.style.overflow = 'hidden';
            
            // Focus first input
            setTimeout(function () {
                const firstInput = modal.querySelector('input:not([type="hidden"]), textarea, select');
                if (firstInput) {
                    firstInput.focus();
                }
            }, Config.animationDuration);
        }
    };

    window.closeModal = function (modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.classList.remove('show');
            document.body.style.overflow = '';
        }
    };

    // ============================================
    // Alert Functions
    // ============================================
    function initAlerts() {
        // Auto-dismiss alerts
        document.querySelectorAll('.alert[data-auto-dismiss]').forEach(function (alert) {
            const delay = parseInt(alert.getAttribute('data-auto-dismiss')) || 5000;
            setTimeout(function () {
                dismissAlert(alert);
            }, delay);
        });

        // Close button
        document.querySelectorAll('.alert-close').forEach(function (btn) {
            btn.addEventListener('click', function () {
                const alert = this.closest('.alert');
                dismissAlert(alert);
            });
        });
    }

    function dismissAlert(alert) {
        if (!alert) return;
        
        alert.style.opacity = '0';
        alert.style.transform = 'translateX(20px)';
        
        setTimeout(function () {
            alert.remove();
        }, Config.animationDuration);
    }

    window.showAlert = function (message, type = 'info', options = {}) {
        const container = document.getElementById('alert-container') || createAlertContainer();
        
        const alert = document.createElement('div');
        alert.className = `alert alert-${type}`;
        
        const iconMap = {
            success: 'fa-check-circle',
            danger: 'fa-exclamation-circle',
            warning: 'fa-exclamation-triangle',
            info: 'fa-info-circle'
        };
        
        alert.innerHTML = `
            <div class="alert-icon">
                <i class="fas ${iconMap[type] || iconMap.info}"></i>
            </div>
            <div class="alert-content">
                ${options.title ? `<h4 class="alert-title">${options.title}</h4>` : ''}
                <p class="alert-message">${message}</p>
            </div>
            <button type="button" class="alert-close">
                <i class="fas fa-times"></i>
            </button>
        `;
        
        container.appendChild(alert);
        
        // Initialize close button
        alert.querySelector('.alert-close').addEventListener('click', function () {
            dismissAlert(alert);
        });
        
        // Auto dismiss
        if (options.autoDismiss !== false) {
            setTimeout(function () {
                dismissAlert(alert);
            }, options.duration || 5000);
        }
        
        return alert;
    };

    function createAlertContainer() {
        const container = document.createElement('div');
        container.id = 'alert-container';
        container.className = 'alert-container';
        document.body.appendChild(container);
        return container;
    }

    // ============================================
    // Tooltip Functions
    // ============================================
    function initTooltips() {
        const tooltipTriggers = document.querySelectorAll('[data-tooltip]');

        tooltipTriggers.forEach(function (trigger) {
            trigger.addEventListener('mouseenter', showTooltip);
            trigger.addEventListener('mouseleave', hideTooltip);
        });
    }

    function showTooltip(e) {
        const trigger = e.currentTarget;
        const text = trigger.getAttribute('data-tooltip');
        const position = trigger.getAttribute('data-tooltip-position') || 'top';

        const tooltip = document.createElement('div');
        tooltip.className = `tooltip tooltip-${position}`;
        tooltip.textContent = text;
        document.body.appendChild(tooltip);

        const triggerRect = trigger.getBoundingClientRect();
        const tooltipRect = tooltip.getBoundingClientRect();

        let top, left;

        switch (position) {
            case 'top':
                top = triggerRect.top - tooltipRect.height - 8;
                left = triggerRect.left + (triggerRect.width - tooltipRect.width) / 2;
                break;
            case 'bottom':
                top = triggerRect.bottom + 8;
                left = triggerRect.left + (triggerRect.width - tooltipRect.width) / 2;
                break;
            case 'left':
                top = triggerRect.top + (triggerRect.height - tooltipRect.height) / 2;
                left = triggerRect.left - tooltipRect.width - 8;
                break;
            case 'right':
                top = triggerRect.top + (triggerRect.height - tooltipRect.height) / 2;
                left = triggerRect.right + 8;
                break;
        }

        tooltip.style.top = `${top + window.scrollY}px`;
        tooltip.style.left = `${left + window.scrollX}px`;

        trigger._tooltip = tooltip;

        requestAnimationFrame(function () {
            tooltip.classList.add('show');
        });
    }

    function hideTooltip(e) {
        const trigger = e.currentTarget;
        const tooltip = trigger._tooltip;

        if (tooltip) {
            tooltip.classList.remove('show');
            setTimeout(function () {
                tooltip.remove();
            }, 200);
        }
    }

    // ============================================
    // Table Functions
    // ============================================
    function initTables() {
        // Select all checkbox
        document.querySelectorAll('.table-select-all').forEach(function (selectAll) {
            selectAll.addEventListener('change', function () {
                const table = this.closest('table');
                const checkboxes = table.querySelectorAll('.table-select-row');
                
                checkboxes.forEach(function (checkbox) {
                    checkbox.checked = selectAll.checked;
                    checkbox.closest('tr').classList.toggle('selected', selectAll.checked);
                });
                
                updateBulkActions(table);
            });
        });

        // Row checkbox
        document.querySelectorAll('.table-select-row').forEach(function (checkbox) {
            checkbox.addEventListener('change', function () {
                const table = this.closest('table');
                const row = this.closest('tr');
                
                row.classList.toggle('selected', this.checked);
                
                // Update select all state
                const selectAll = table.querySelector('.table-select-all');
                const allCheckboxes = table.querySelectorAll('.table-select-row');
                const checkedCheckboxes = table.querySelectorAll('.table-select-row:checked');
                
                if (selectAll) {
                    selectAll.checked = allCheckboxes.length === checkedCheckboxes.length;
                    selectAll.indeterminate = checkedCheckboxes.length > 0 && checkedCheckboxes.length < allCheckboxes.length;
                }
                
                updateBulkActions(table);
            });
        });

        // Sortable columns
        document.querySelectorAll('.table th.sortable').forEach(function (th) {
            th.addEventListener('click', function () {
                const table = this.closest('table');
                const columnIndex = Array.from(this.parentElement.children).indexOf(this);
                const currentOrder = this.classList.contains('asc') ? 'desc' : 'asc';
                
                // Remove sort classes from other columns
                table.querySelectorAll('th.sortable').forEach(function (col) {
                    col.classList.remove('asc', 'desc');
                });
                
                this.classList.add(currentOrder);
                
                // Sort table rows
                sortTable(table, columnIndex, currentOrder);
            });
        });

        // Action dropdowns
        document.querySelectorAll('.table-actions-dropdown').forEach(function (dropdown) {
            const trigger = dropdown.querySelector('.table-action-btn');
            
            if (trigger) {
                trigger.addEventListener('click', function (e) {
                    e.stopPropagation();
                    
                    // Close other dropdowns
                    document.querySelectorAll('.table-actions-dropdown.open').forEach(function (d) {
                        if (d !== dropdown) {
                            d.classList.remove('open');
                        }
                    });
                    
                    dropdown.classList.toggle('open');
                });
            }
        });

        // Close action dropdowns on click outside
        document.addEventListener('click', function () {
            document.querySelectorAll('.table-actions-dropdown.open').forEach(function (dropdown) {
                dropdown.classList.remove('open');
            });
        });
    }

    function updateBulkActions(table) {
        const container = table.closest('.table-container');
        const bulkActions = container?.querySelector('.table-bulk-actions');
        const selectedCount = table.querySelectorAll('.table-select-row:checked').length;
        
        if (bulkActions) {
            const countElement = bulkActions.querySelector('.table-bulk-count');
            
            if (selectedCount > 0) {
                bulkActions.classList.add('show');
                if (countElement) {
                    countElement.textContent = `${selectedCount} item${selectedCount > 1 ? 's' : ''} selected`;
                }
            } else {
                bulkActions.classList.remove('show');
            }
        }
    }

    function sortTable(table, columnIndex, order) {
        const tbody = table.querySelector('tbody');
        const rows = Array.from(tbody.querySelectorAll('tr'));
        
        rows.sort(function (a, b) {
            const aText = a.cells[columnIndex]?.textContent.trim() || '';
            const bText = b.cells[columnIndex]?.textContent.trim() || '';
            
            // Try to compare as numbers
            const aNum = parseFloat(aText.replace(/[^0-9.-]/g, ''));
            const bNum = parseFloat(bText.replace(/[^0-9.-]/g, ''));
            
            if (!isNaN(aNum) && !isNaN(bNum)) {
                return order === 'asc' ? aNum - bNum : bNum - aNum;
            }
            
            // Compare as strings
            return order === 'asc' 
                ? aText.localeCompare(bText) 
                : bText.localeCompare(aText);
        });
        
        rows.forEach(function (row) {
            tbody.appendChild(row);
        });
    }

    // ============================================
    // Form Functions
    // ============================================
    function initForms() {
        // Character counter
        document.querySelectorAll('[data-char-counter]').forEach(function (input) {
            const maxLength = parseInt(input.getAttribute('maxlength'));
            const counterId = input.getAttribute('data-char-counter');
            const counter = document.getElementById(counterId);
            
            if (counter && maxLength) {
                function updateCounter() {
                    const remaining = maxLength - input.value.length;
                    counter.textContent = `${input.value.length} / ${maxLength}`;
                    
                    counter.classList.remove('warning', 'danger');
                    if (remaining <= 10) {
                        counter.classList.add('danger');
                    } else if (remaining <= 20) {
                        counter.classList.add('warning');
                    }
                }
                
                input.addEventListener('input', updateCounter);
                updateCounter();
            }
        });

        // Form validation styling
        document.querySelectorAll('form[data-validate]').forEach(function (form) {
            form.addEventListener('submit', function (e) {
                if (!form.checkValidity()) {
                    e.preventDefault();
                    e.stopPropagation();
                    
                    // Add validation classes
                    form.querySelectorAll(':invalid').forEach(function (input) {
                        input.classList.add('is-invalid');
                    });
                    
                    // Focus first invalid input
                    const firstInvalid = form.querySelector(':invalid');
                    if (firstInvalid) {
                        firstInvalid.focus();
                    }
                }
                
                form.classList.add('was-validated');
            });
            
            // Remove invalid class on input
            form.querySelectorAll('.form-control, .form-select').forEach(function (input) {
                input.addEventListener('input', function () {
                    if (this.checkValidity()) {
                        this.classList.remove('is-invalid');
                        this.classList.add('is-valid');
                    }
                });
            });
        });

        // File dropzone
        document.querySelectorAll('.dropzone').forEach(function (dropzone) {
            const input = dropzone.querySelector('input[type="file"]');
            
            ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(function (eventName) {
                dropzone.addEventListener(eventName, function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                });
            });
            
            ['dragenter', 'dragover'].forEach(function (eventName) {
                dropzone.addEventListener(eventName, function () {
                    dropzone.classList.add('dragover');
                });
            });
            
            ['dragleave', 'drop'].forEach(function (eventName) {
                dropzone.addEventListener(eventName, function () {
                    dropzone.classList.remove('dragover');
                });
            });
            
            dropzone.addEventListener('drop', function (e) {
                const files = e.dataTransfer.files;
                if (input && files.length > 0) {
                    input.files = files;
                    input.dispatchEvent(new Event('change'));
                }
            });
            
            dropzone.addEventListener('click', function () {
                if (input) {
                    input.click();
                }
            });
        });

        // Password toggle
        document.querySelectorAll('[data-password-toggle]').forEach(function (toggle) {
            toggle.addEventListener('click', function () {
                const inputId = this.getAttribute('data-password-toggle');
                const input = document.getElementById(inputId);
                const icon = this.querySelector('i');
                
                if (input) {
                    if (input.type === 'password') {
                        input.type = 'text';
                        icon?.classList.replace('fa-eye', 'fa-eye-slash');
                    } else {
                        input.type = 'password';
                        icon?.classList.replace('fa-eye-slash', 'fa-eye');
                    }
                }
            });
        });
    }

    // ============================================
    // Theme Functions
    // ============================================
    function initTheme() {
        const themeToggle = document.querySelector('[data-theme-toggle]');
        const savedTheme = localStorage.getItem(Config.themeKey);
        
        if (savedTheme) {
            document.documentElement.setAttribute('data-theme', savedTheme);
        }
        
        if (themeToggle) {
            themeToggle.addEventListener('click', function () {
                const currentTheme = document.documentElement.getAttribute('data-theme');
                const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
                
                document.documentElement.setAttribute('data-theme', newTheme);
                localStorage.setItem(Config.themeKey, newTheme);
                
                // Update icon
                const icon = this.querySelector('i');
                if (icon) {
                    icon.classList.toggle('fa-moon', newTheme === 'light');
                    icon.classList.toggle('fa-sun', newTheme === 'dark');
                }
            });
        }
    }

    // ============================================
    // Search Functions
    // ============================================
    function initSearch() {
        const searchInput = document.querySelector('.header-search-input');
        
        if (searchInput) {
            // Keyboard shortcut (Ctrl/Cmd + K)
            document.addEventListener('keydown', function (e) {
                if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
                    e.preventDefault();
                    searchInput.focus();
                }
            });
            
            // Search on enter
            searchInput.addEventListener('keypress', function (e) {
                if (e.key === 'Enter') {
                    e.preventDefault();
                    const query = this.value.trim();
                    if (query) {
                        window.location.href = `/search?q=${encodeURIComponent(query)}`;
                    }
                }
            });
        }
    }

    // ============================================
    // Utility Functions
    // ============================================
    window.AppUtils = {
        formatNumber: function (num) {
            return new Intl.NumberFormat().format(num);
        },
        
        formatCurrency: function (amount, currency = 'USD') {
            return new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: currency
            }).format(amount);
        },
        
        formatDate: function (date, options = {}) {
            return new Intl.DateTimeFormat('en-US', {
                dateStyle: options.dateStyle || 'medium',
                timeStyle: options.timeStyle
            }).format(new Date(date));
        },
        
        debounce: function (func, wait) {
            let timeout;
            return function executedFunction(...args) {
                const later = function () {
                    clearTimeout(timeout);
                    func(...args);
                };
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
            };
        },
        
        throttle: function (func, limit) {
            let inThrottle;
            return function executedFunction(...args) {
                if (!inThrottle) {
                    func(...args);
                    inThrottle = true;
                    setTimeout(function () {
                        inThrottle = false;
                    }, limit);
                }
            };
        },
        
        copyToClipboard: function (text) {
            return navigator.clipboard.writeText(text).then(function () {
                showAlert('Copied to clipboard!', 'success', { duration: 2000 });
            });
        }
    };

})();
