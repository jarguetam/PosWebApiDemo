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
                    builder.WithOrigins("https://diproal-app.onrender.com",
                        "http://192.168.10.10:9096"
                        );
                    builder.WithMethods("*");
                    builder.WithHeaders("*");
                    builder.SetIsOriginAllowedToAllowWildcardSubdomains();
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
            services.AddTokenAuthentication(Configuration);
            services.AddControllers();
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
            //app.UseSentryTracing();

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
