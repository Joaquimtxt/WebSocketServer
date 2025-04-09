using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using WebSocketServer.Hubs;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Adiciona SignalR
        services.AddSignalR();

        // Configuração do CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => builder
                    .AllowAnyOrigin() // Permite qualquer origem (React, etc.)
                    .AllowAnyMethod() // Permite qualquer método HTTP
                    .AllowAnyHeader()); // Permite qualquer cabeçalho
        });

        // Configuração do Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
            loggingBuilder.AddSerilog(dispose: true));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Redireciona para HTTPS (opcional, remova se não estiver usando HTTPS)
        app.UseHttpsRedirection();

        app.UseRouting();

        // Aplica a política de CORS
        app.UseCors("AllowAll");

        app.UseAuthorization();

        // Mapeia o endpoint do SignalR
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<ChatHub>("/chathub");
        });
    }
}