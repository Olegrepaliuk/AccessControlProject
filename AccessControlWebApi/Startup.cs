using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessControl.Models;
using AccessControlModels;
using AccessControlWebApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AccessControlWebApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AccessCtrlContext>(options => options.UseSqlServer(connection));
            //services.AddControllersWithViews();

            services.AddDbContext<AppUsersContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("UsersConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AppUsersContext>();
            services.AddScoped<ControlService>();
            services.AddScoped<ControlRepository>();
            services.AddScoped<UserService>();
            services.AddScoped<UserRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseAuthentication();   how 
            //app.UseAuthorization();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
