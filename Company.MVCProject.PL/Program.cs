using Company.MVCProject.BLL;
using Company.MVCProject.BLL.Interfaces;
using Company.MVCProject.BLL.Repositories;
using Company.MVCProject.DAL.Data.Contexts;
using Company.MVCProject.DAL.Models;
using Company.MVCProject.PL.Helpers.Email;
using Company.MVCProject.PL.Helpers.Sms;
using Company.MVCProject.PL.Mapping;
using Company.MVCProject.PL.Services;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Company.MVCProject.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (builder.Environment.IsDevelopment())
            {     
                builder.Configuration.AddUserSecrets<Program>(optional: true);
            }


            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<CompanyDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            //builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            //builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            //builder.Services.AddAutoMapper(M => M.AddProfile(typeof(EmployeeProfile)));

            builder.Services.AddAutoMapper(M => M.AddProfile(new EmployeeProfile()));
            builder.Services.AddAutoMapper(M => M.AddProfile(new DepartmentProfile()));

            //builder.Services.AddScoped(); // Create Object Life Time Per Request - UnReachable Object
            //builder.Services.AddTransient(); // Create Object Life Time Per Operation
            //builder.Services.AddSingleton(); // Create Object Life Time Per Application

            builder.Services.AddScoped<IScopedService, ScopedService>(); // Per Request
            builder.Services.AddTransient<ITransientService, TransientService>(); // Per Operation
            builder.Services.AddSingleton<ISingletonService, SingletonService>(); // Per Application

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                            .AddEntityFrameworkStores<CompanyDbContext>()
                            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(config =>
            {
                config.LoginPath = "/Account/SignIn";
            });

            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));

            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddScoped<ITwilioServices, TwilioServices>();

            builder.Services.AddAuthentication().AddGoogle(o =>
            {
                o.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                o.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });


            //builder.Services.AddAuthentication(o =>
            //{
            //}).AddFacebook(o =>
            //{
            //    o.ClientId = builder.Configuration["Authentication:Facebook:ClientId"];
            //    o.ClientId = builder.Configuration["Authentication:Facebook:ClientSecret"];
            //});


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
