using Microsoft.OpenApi.Models;
using Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper;
using Application.Core;
using Application.Activities;
using Application.interfaces;
using Infrastructure.Security;
using Infrastructure.Photos;
using Application.Interfaces;

namespace API.Extensions
{
 public static class ApplicationServiceExtensions
 {
  public static IServiceCollection AddApplicationServices(this IServiceCollection services,
      IConfiguration config)
  {
   services.AddSwaggerGen(c =>
   {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv5", Version = "v1" });
   });
   services.AddDbContext<DataContext>(opt =>
   {
    opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
   });
   // add Cors is required when trying to access resource from a different domain
   // client app: localhost port 3000, while API server localhost port 5000
   // return header with response to allow any method, get, posts, put, option, etc.
   // with origin of client app
   // if publish online, AddCors become irrelevant because client app will request from the same domain
   services.AddCors(opt =>
   {
    opt.AddPolicy("CorsPolicy", policy =>
             {
              policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
             });
   });
   services.AddMediatR(typeof(List.Handler).Assembly); // tell mediatR where to find the handler
   services.AddAutoMapper(typeof(MappingProfiles).Assembly);
   services.AddScoped<IUserAccessor, UserAccessor>();
   services.AddScoped<IPhotoAccessor, PhotoAccessor>();
   services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));

   return services;
  }
 }
}
