using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TenantService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "modules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "plan_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_sub_category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_sub_category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_services_modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "plan_subcategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PlanCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_subcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_plan_subcategories_plan_categories_PlanCategoryId",
                        column: x => x.PlanCategoryId,
                        principalTable: "plan_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CountryId = table.Column<string>(type: "text", nullable: true),
                    CompanyName = table.Column<string>(type: "text", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    AddressLineOne = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AddressLineTwo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PhoneCountryCode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Logo = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    Blacklisted = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SubDomain = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TenantCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tenants_tenant_category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "tenant_category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenants_tenant_sub_category_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "tenant_sub_category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "service_attributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AttributeCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_attributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_service_attributes_services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DurationValue = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CostAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CostCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    IsCustom = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_plans_plan_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "plan_categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_plans_plan_subcategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "plan_subcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_credentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Platform = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    KeyType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    KeyName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    KeyValueHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_credentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tenant_credentials_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "plan_services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceAttributeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeValue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_plan_services_plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_plan_services_service_attributes_ServiceAttributeId",
                        column: x => x.ServiceAttributeId,
                        principalTable: "service_attributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_plan_services_services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subscription",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    BillingCycle = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subscription_plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subscription_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "usage_tracking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentUsage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usage_tracking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usage_tracking_plan_services_PlanServiceId",
                        column: x => x.PlanServiceId,
                        principalTable: "plan_services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_usage_tracking_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BillingStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BillingEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tenant_payment_subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenant_payment_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_modules_CreatedAt",
                table: "modules",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_modules_ModuleCode",
                table: "modules",
                column: "ModuleCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_modules_Name",
                table: "modules",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_plan_categories_CreatedAt",
                table: "plan_categories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_plan_services_CreatedAt",
                table: "plan_services",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_plan_services_PlanId",
                table: "plan_services",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_plan_services_ServiceAttributeId",
                table: "plan_services",
                column: "ServiceAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_plan_services_ServiceId",
                table: "plan_services",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_plan_subcategories_CreatedAt",
                table: "plan_subcategories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_plan_subcategories_PlanCategoryId",
                table: "plan_subcategories",
                column: "PlanCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_plans_CategoryId",
                table: "plans",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_plans_CreatedAt",
                table: "plans",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_plans_SubCategoryId",
                table: "plans",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_service_attributes_CreatedAt",
                table: "service_attributes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAttributes_ServiceId_AttributeCode",
                table: "service_attributes",
                columns: new[] { "ServiceId", "AttributeCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAttributes_ServiceId_Name",
                table: "service_attributes",
                columns: new[] { "ServiceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_services_CreatedAt",
                table: "services",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_services_ModuleId",
                table: "services",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_services_Name",
                table: "services",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_services_ServiceCode",
                table: "services",
                column: "ServiceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscription_CreatedAt",
                table: "subscription",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_PlanId",
                table: "subscription",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_TenantId",
                table: "subscription",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_category_CreatedAt",
                table: "tenant_category",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_category_Name",
                table: "tenant_category",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenant_credentials_CreatedAt",
                table: "tenant_credentials",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_credentials_TenantId",
                table: "tenant_credentials",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_payment_CreatedAt",
                table: "tenant_payment",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_payment_SubscriptionId",
                table: "tenant_payment",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_payment_TenantId",
                table: "tenant_payment",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_sub_category_CreatedAt",
                table: "tenant_sub_category",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_sub_category_Name",
                table: "tenant_sub_category",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_CategoryId",
                table: "tenants",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_CreatedAt",
                table: "tenants",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_SubCategoryId",
                table: "tenants",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_usage_tracking_CreatedAt",
                table: "usage_tracking",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_usage_tracking_PlanServiceId",
                table: "usage_tracking",
                column: "PlanServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_usage_tracking_TenantId",
                table: "usage_tracking",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenant_credentials");

            migrationBuilder.DropTable(
                name: "tenant_payment");

            migrationBuilder.DropTable(
                name: "usage_tracking");

            migrationBuilder.DropTable(
                name: "subscription");

            migrationBuilder.DropTable(
                name: "plan_services");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "plans");

            migrationBuilder.DropTable(
                name: "service_attributes");

            migrationBuilder.DropTable(
                name: "tenant_category");

            migrationBuilder.DropTable(
                name: "tenant_sub_category");

            migrationBuilder.DropTable(
                name: "plan_subcategories");

            migrationBuilder.DropTable(
                name: "services");

            migrationBuilder.DropTable(
                name: "plan_categories");

            migrationBuilder.DropTable(
                name: "modules");
        }
    }
}

