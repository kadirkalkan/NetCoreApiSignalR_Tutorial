using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreSignalR.Signalr;

namespace NetCoreSignalR
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
            services.AddControllers();
            // Cors Middleware'i ile gelen istekler yönetilir. Aþaðýdaki ayarlar ile Header, Method, Ýmza farketmeksizin bütün istekler kabul edilir.
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                .WithOrigins("http://localhost:4200") //.AllowAnyOrigin() -- Kullanýlabilecek diðer methoddur bütün url'lerden eriþime izin verir. Fakat AllowCreditentals ile kullanýlamaz.
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });
            // SignalR middleware'i ile signalR'ý etkinleþtiriyoruz
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Cors ayarlarý uygulanýyor.
            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            //Endpointler url/.... ile ulaþýlacak methodun etiketlendiði yerdir.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ApplicationHub>("/applicationHub");
            });
        }
    }
}
