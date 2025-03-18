using Pos.WebApi.Features.Auth;
using Pos.WebApi.Features.Common;
using Pos.WebApi.Features.Users;
using Pos.WebApi.Features.Users.Services;
using Pos.WebApi.Helpers;
using Pos.WebApi.Infraestructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pos.WebApi.Features.Items.Services;
using Pos.WebApi.Features.Sellers.Services;
using Pos.WebApi.Features.Customers.Services;
using Pos.WebApi.Features.Suppliers.Services;
using Pos.WebApi.Features.InventoryTransactions.Services;
using Pos.WebApi.Features.Purchase.Services;
using Pos.WebApi.Features.PurchasePayment.Service;
using Pos.WebApi.Features.Sales.Services;
using Pos.WebApi.Features.SalesPayment.Service;
using Pos.WebApi.Features.Dashboard;
using Pos.WebApi.Features.Reports.Services;
using Pos.WebApi.Features.Expenses.Services;
using Pos.WebApi.Features.Liquidations.Services;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Pos.WebApi
{
    public class Startup
    {
        readonly string MiCors = "Micors";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pos.WebApi", Version = "v1" });
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "JWT Bearer Token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[]{ } }
                });
            });

            services.AddDbContext<PosDbContext>(
                dbContextOptions => dbContextOptions
                    .UseSqlServer(Configuration.GetConnectionString("dbpos"))
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );

            services.AddCors(options =>
            options.AddPolicy(name: MiCors,
                builder =>
                {
                    builder
                     .AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader();
                })
            );

            services.AddTransient<AuthService, AuthService>();
            services.AddTransient<UserService, UserService>();
            services.AddTransient<CommonService, CommonService>();
            services.AddTransient<RoleService, RoleService>();
            services.AddTransient<PermissionService, PermissionService>();
            services.AddTransient<WareHouseServices, WareHouseServices>();
            services.AddTransient<SellerServices, SellerServices>();
            services.AddTransient<CustomerServices, CustomerServices>();
            services.AddTransient<SupplierServices, SupplierServices>();
            services.AddTransient<ItemServices, ItemServices>();
            services.AddTransient<ItemJournalServices, ItemJournalServices>();
            services.AddTransient<BPJornalServices, BPJornalServices>();
            services.AddTransient<InventoryTransactionServices,InventoryTransactionServices>();
            services.AddTransient<PurchaseServices, PurchaseServices>();
            services.AddTransient<PaymentPurchaseServices, PaymentPurchaseServices>();
            services.AddTransient<SalesServices, SalesServices>();
            services.AddTransient<PaymentSaleServices, PaymentSaleServices>();
            services.AddTransient<DashboardServices, DashboardServices>();
            services.AddTransient<ReportServices, ReportServices>();
            services.AddTransient<ExpenseServices, ExpenseServices>();
            services.AddTransient<LiquidationServices, LiquidationServices>();          
            services.AddTransient<GenericDataService, GenericDataService>();
            services.AddMemoryCache();
            services.AddTokenAuthentication(Configuration);

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                
            }
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pos.WebApi v1"));
            app.UseHttpsRedirection();

            app.UseRouting();

            //Habilitamos los cords para usar en la web OJO Solo en pruebas en produccion hay que especificiar el origen
            app.UseCors(x => x
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pos.WebApi v1"));
            app.UseAuthentication();
            app.UseAuthorization();
       

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
