using Microsoft.OpenApi.Models;

namespace JsonWebTokensAPI
{
    public class Startup
    {
        //TODO: Ver cómo implementar el Middleware
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //Carga de la configuración de la Base de datos
            //Configuracion configuracion = new Configuracion();
            //Configuration.GetSection("JWT").Bind(configuracion);
            //Configuration.GetSection("BaseDatos").Bind(configuracion);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Controladores de API
            services.AddControllers();

            //Información sobre la API
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JsonWebTokensAPI", Version = "v1" });
            });

            //CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:44496").AllowAnyHeader().AllowAnyMethod();
                    });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JsonWebTokensAPI v1"));
            }

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
