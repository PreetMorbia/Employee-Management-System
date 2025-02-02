using Enterprise.EmployeeManagement.BAL.Repository;
using Enterprise.EmployeeManagement.DAL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;


namespace Enterprise.EmployeeManagementSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add DbContext service for MySQL database connection
            services.AddDbContext<AppDbContext>(options =>
                 options.UseMySql(Configuration.GetConnectionString("DefaultDatabase")));


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options => {options.ExpireTimeSpan = new TimeSpan(0, 30, 0);
                options.LoginPath = "/Employee/LoginPage";
                options.AccessDeniedPath = "/Home/AccessDenied";
                });
            
            // Add controllers with views if needed
            services.AddControllersWithViews();

            services.AddScoped<EmployeeRepository>();
            services.AddScoped<TaskOperationsRepository>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                
            }

            // Serve static files like CSS, JS, images, etc.
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            // Setup the default route
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Employee}/{action=LoginPage}/{id?}");
            });
        }
    }
}
