using FAQChatBot.Contexts;
using FAQChatBot.Interfaces;
using FAQChatBot.Models;
using FAQChatBot.Models.Config;
using FAQChatBot.Misc;
using FAQChatBot.Repositories;
using FAQChatBot.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                    opts.JsonSerializerOptions.WriteIndented = true;
                });

builder.Services.AddDbContext<FAQContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddTransient<IRepository<int, FAQ>, FAQRepository>();
builder.Services.AddTransient<IRepository<int, ChatLog>, ChatLogRepository>();
builder.Services.AddTransient<IRepository<int, User>, UserRepository>();

builder.Services.AddTransient<IFAQService, FAQService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.Configure<HuggingFaceOptions>(builder.Configuration.GetSection("HuggingFace"));
// builder.Services.AddTransient<IOtherContextFunctionalities, OtherFunctionalitiesImplementation>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();