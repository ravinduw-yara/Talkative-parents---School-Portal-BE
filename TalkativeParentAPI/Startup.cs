using CommonUtility;
using CommonUtility.CommonModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Repository.DBContext;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkativeParentAPI.Installers;
using UploaderSheet_StudentMark.DataAccessLayer;

namespace TalkativeParentAPI
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
            services.AddScoped<IUploadFileDL, UploadFileDL>();
            services.InstallServicesAssembly(Configuration);

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            services.AddSingleton(x =>
            {
                var config = x.GetRequiredService<IConfiguration>();
                var connectionString = config.GetValue<string>("AzureBlobStorage:ConnectionString");
                var containerName = config.GetValue<string>("AzureBlobStorage:ContainerName");
                var blobName = config.GetValue<string>("AzureBlobStorage:BlobName");

                if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(containerName) || string.IsNullOrWhiteSpace(blobName))
                    throw new InvalidOperationException("Azure Blob Storage configuration is missing or invalid.");

                return new GoogleDriveService(connectionString, containerName, blobName);
            });
            services.AddSingleton(x =>
            {
                var config = x.GetRequiredService<IConfiguration>();
                var connectionString = config.GetValue<string>("AzureBlobStorage:ConnectionString");
                var containerName = config.GetValue<string>("AzureBlobStorage:ContainerName");
                var blobName = config.GetValue<string>("AzureBlobStorage:BlobName");

                if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(containerName) || string.IsNullOrWhiteSpace(blobName))
                    throw new InvalidOperationException("Azure Blob Storage configuration is missing or invalid.");

                return new MUploadPdfGoogleDriveService(connectionString, containerName, blobName);
            });
            services.AddSingleton(x =>
            {
                var config = x.GetRequiredService<IConfiguration>();
                var connectionString = config.GetValue<string>("AzureBlobStorage:ConnectionString");
                var containerName = config.GetValue<string>("AzureBlobStorage:ContainerName");
                var blobName = config.GetValue<string>("AzureBlobStorage:BlobName");

                if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(containerName) || string.IsNullOrWhiteSpace(blobName))
                    throw new InvalidOperationException("Azure Blob Storage configuration is missing or invalid.");

                return new MUploadPdfSyllabusGoogleDriveService(connectionString, containerName, blobName);
            });

            //Added by Ranjan
            services
               .AddMvc(options => options.EnableEndpointRouting = false)
               .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
               .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddControllers();
            services.AddDbContext<TpContext>(opt => {
                opt.UseSqlServer(Configuration.GetConnectionString("TpConnectionString"));
            });


            services.AddSwaggerGen(c =>
            {
                // Add JWT Authentication
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = "bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            });

            //JWT KEY IS REQUIRED

            services.Configure<Jwt>(Configuration.GetSection("Jwt"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
                  {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = Configuration["Jwt:Issuer"],
                         ValidAudience = Configuration["Jwt:Issuer"],
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                     };
               });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
                {
                    context.Request.Path = "/";
                    await next();
                }
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API WebApp");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseDeveloperExceptionPage();

            app.UseRouting();


            app.UseCors("CorsPolicy");

            app.UseMvc();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
