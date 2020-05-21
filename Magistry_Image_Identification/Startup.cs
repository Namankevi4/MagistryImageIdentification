using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Configuration;
using DatabaseLogic;
using Infrasctructure.OpenCvFunctionality;
using Infrasctructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Magistry_Image_Identification
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
            services.AddDirectoryBrowser();
            services.AddControllersWithViews();

            services.Configure<ConfigSettings>(Configuration.GetSection("ApplicationSettings"));

            services.AddSingleton<DescriptorMatherWrapper>();
            services.AddTransient<ImageDatabase>();

            services.AddScoped<ImageService>();
            services.AddScoped<OpenCVMeasures>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            var pathToImageFolder = Configuration.GetSection("ApplicationSettings").GetValue<string>("PathToImagesFolder");

            Directory.CreateDirectory(Path.Combine(env.WebRootPath, pathToImageFolder));

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pathToImageFolder)),
                RequestPath = $"/{pathToImageFolder}"
            });


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            StartupApp(app.ApplicationServices);
        }

        private void StartupApp(IServiceProvider serviceProvider)
        {
            

            var imageDatabase = serviceProvider.GetService<ImageDatabase>();

            var imagesForTraining = Task.Run(async () => await imageDatabase.GetAll()).Result;

            for (int i = 0; i < imagesForTraining.Count; i++)
            {
                if (!File.Exists(imagesForTraining[0].ImagePath))
                {
                    Task.Run(async () => await imageDatabase.DeleteByPath(imagesForTraining[0].ImagePath)).Wait();
                    imagesForTraining.Remove(imagesForTraining[0]);
                }
            }

            serviceProvider.GetService<DescriptorMatherWrapper>().Startup(imagesForTraining);
        }
    }
}
