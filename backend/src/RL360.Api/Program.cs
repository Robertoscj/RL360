using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RL360.Api.Middleware;
using RL360.Api.Realtime;
using RL360.Api.Security;
using RL360.Application;
using RL360.Application.Abstractions;
using RL360.Infrastructure;
using RL360.Infrastructure.Security;
using RL360.Shared.Constants;
using RL360.Shared.Options;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

builder.Services.AdicionarAplicacao();
builder.Services.AdicionarInfraestrutura(builder.Configuration);
builder.Services.Configure<ConfiguracaoIa>(builder.Configuration.GetSection("Ia"));
builder.Services.AddHttpClient("OpenAI", client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddScoped<IUsuarioAtual, UsuarioAtual>();
builder.Services.AddScoped<NotificadorDashboardHub>();
builder.Services.AddScoped<IDashboardNotificador>(sp => sp.GetRequiredService<NotificadorDashboardHub>());
builder.Services.AddScoped<IRadarNotificador>(sp => sp.GetRequiredService<NotificadorDashboardHub>());

var jwt = builder.Configuration.GetSection("Jwt").Get<ConfiguracaoJwt>() ?? new ConfiguracaoJwt();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Emissor,
            ValidAudience = jwt.Audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.ChaveSecreta)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var accessToken = ctx.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) &&
                    ctx.HttpContext.Request.Path.StartsWithSegments(CanaisTempoReal.CaminhoHubRadar))
                {
                    ctx.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

const string PoliticaCors = "rl360-frontend";
builder.Services.AddCors(options =>
{
    options.AddPolicy(PoliticaCors, policy =>
    {
        var origens = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                      ?? ["http://localhost:5173", "http://localhost:5190", "http://localhost:3000"];
        policy.WithOrigins(origens)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RL360 API", Version = "v1", Description = "Radar de Lucro em Tempo Real" });
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = Array.Empty<string>() });
});

var app = builder.Build();

app.UseMiddleware<TratamentoExcecoesMiddleware>();
app.UseMiddleware<LogRequisicaoMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RL360 API v1"));

app.UseCors(PoliticaCors);
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ValidacaoEmpresaMiddleware>();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", servico = "RL360.Api", hora = DateTime.UtcNow }))
    .AllowAnonymous();

app.MapControllers();
app.MapHub<HubDashboard>(CanaisTempoReal.CaminhoHubDashboard);

app.Run();
