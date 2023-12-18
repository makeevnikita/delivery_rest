using Microsoft.EntityFrameworkCore;
using delivery.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using delivery.Helpers;



namespace delivery
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {   
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtOptions =>
            {
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = AuthHelper.GetKey(),
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });

            services.AddAuthorization();

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = 50; //default 1024
                options.MultipartBodyLengthLimit = 50 * 1024 * 1024;
            });
            string connectionString = Configuration.GetConnectionString("PostgresConnection");

            services.AddDbContext<DeliveryContext>(options =>
                options.UseNpgsql(connectionString));
                
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {   

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {   
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "GetProductById",
                    pattern: "product/get/{id}"
                );
                endpoints.MapControllerRoute(
                    name: "GetProductCategoryById",
                    pattern: "admin/product_category/get/{id}"
                );
                endpoints.MapControllerRoute(
                    name: "AdminGetProductCategoryById",
                    pattern: "admin/product_category/get/{id}"
                );
                endpoints.MapControllerRoute(
                    name: "AdminGetModificatorById",
                    pattern: "admin/modificator/get/{id}"
                );
                endpoints.MapControllerRoute(
                    name: "GetOrderById",
                    pattern: "manager/order/get/{id}"
                );
            });
        }
    }
}
