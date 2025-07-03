using System.Text;
using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Repositories;
using Freelance_Project.Models;
using Freelance_Project.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Freelance_Project.Misc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.SignalR;

#region Logging Configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Set base level
    .WriteTo.Console(
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information // Console: log Info and above
    )
   .WriteTo.File(
        "Logs/app-log.txt",
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, // File: log only Error and above
        rollingInterval: RollingInterval.Day
    )
    .CreateLogger();
#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
#region Swagger Configuration
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Clinic API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
#endregion
#region Controller Configuration
builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.Converters.Add(new TimeSpanConverter());
            // opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            opts.JsonSerializerOptions.WriteIndented = true;
        });
#endregion
#region API Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("PerUserPolicy", context =>
    {
        var userEmail = context.User?.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        var partitionKey = !string.IsNullOrEmpty(userEmail)
            ? userEmail
            : (context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous");

        var permitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit", 1000); // Default to 1000 if not set
        var window = TimeSpan.FromHours(1);

        return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ =>
            new TokenBucketRateLimiterOptions
            {
                TokenLimit = permitLimit,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0,
                ReplenishmentPeriod = window,
                TokensPerPeriod = permitLimit,
                AutoReplenishment = true
            });
    });
});
#endregion

#region SignalR Configuration
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUsers", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "http://localhost:4201", "https://localhost:8080", "http://localhost:8081")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
#endregion

#region Configure Kestrel server to use TLS 1.2
// Only configure Kestrel HTTPS in production
if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ConfigureHttpsDefaults(httpsOptions =>
        {
            httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        });
    });
}
#endregion

#region Database Context
builder.Services.AddDbContext<FreelanceDBContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
#endregion

#region Repositories
builder.Services.AddTransient<IRepository<Guid, Client>, ClientRepository>();
builder.Services.AddTransient<IRepository<Guid, Freelancer>, FreelancerRepository>();
builder.Services.AddTransient<IRepository<Guid, Project>, ProjectRepository>();
builder.Services.AddTransient<IRepository<Guid, Proposal>, ProposalRepository>();
builder.Services.AddTransient<IRepository<Guid, Skill>, SkillRepository>();
builder.Services.AddTransient<IRepository<string, User>, UserRepository>();
builder.Services.AddTransient<IRepository<Guid, ChatRoom>, ChatRoomRepository>();
builder.Services.AddTransient<IRepository<Guid, ChatMessage>, ChatMessageRepository>();
#endregion

#region Services
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IHashingService, HashingService>();
builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<IFreelancerService, FreelancerService>();
builder.Services.AddTransient<IClientProjectService, ClientProjectService>();
builder.Services.AddTransient<IFreelancerProposalService, FreelancerProposalService>();
builder.Services.AddTransient<IProjectProposalService, ProjectProposalService>();
builder.Services.AddTransient<IImageUploadService, ImageUploadService>();
builder.Services.AddTransient<IChatService, ChatService>();
#endregion

#region Image Upload Configuration
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddTransient<ImageUploadService>();

#endregion

#region JWT Authentication Service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Unauthorized",
                    data = (object)null,
                    errors = new { Authorization = new[] { "Authentication token is missing or invalid." } }
                });
                return context.Response.WriteAsync(result);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Forbidden",
                    data = (object)null,
                    errors = new { Authorization = new[] { "You do not have permission to access this resource." } }
                });
                return context.Response.WriteAsync(result);
            },
            OnMessageReceived = context =>
            {
                // Allow JWT in query string for SignalR
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationhub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ClockSkew = TimeSpan.Zero
        };
    });
#endregion

#region API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
#endregion

#region Misc
builder.Services.AddTransient<IGetOrCreateSkills, GetOrCreateSkillService>();
#endregion

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowUsers");
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationhub");

app.Run();
