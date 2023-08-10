using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services,
      IConfiguration config)
  {
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    //builder.Services.AddEndpointsApiExplorer();

    services.AddCors();

    services.AddScoped<ITokenService, TokenService>();
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    services.Configure<CloudniarySettings>(config.GetSection("CloudniarySettings"));
    services.AddScoped<IPhotoService, PhotoService>();
    services.AddScoped<LogUserActivity>();
    services.AddSignalR();
    services.AddSingleton<PresenseTracker>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    return services;
  }
}
