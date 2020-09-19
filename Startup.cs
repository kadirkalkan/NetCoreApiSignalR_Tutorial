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
            // Cors Middleware'i ile gelen istekler y�netilir. A�a��daki ayarlar ile Header, Method, �mza farketmeksizin b�t�n istekler kabul edilir.
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                .WithOrigins("http://localhost:4200") //.AllowAnyOrigin() -- Kullan�labilecek di�er methoddur b�t�n url'lerden eri�ime izin verir. Fakat AllowCreditentals ile kullan�lamaz.
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });
            // SignalR middleware'i ile signalR'� etkinle�tiriyoruz
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Cors ayarlar� uygulan�yor.
            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            //Endpointler url/.... ile ula��lacak methodun etiketlendi�i yerdir.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ApplicationHub>("/applicationHub");
            });
        }
    }
}
