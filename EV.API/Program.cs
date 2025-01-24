using EnVietSocialNetWorkAPI.Services;
using EV.Common.Helpers.Authentication;
using EV.Common.Middlewares;
using EV.Common.Services.Email;
using EV.Common.Services.UploadFile;
using EV.Common.Services.UploadFile.Interfaces;
using EV.Common.SettingConfigurations;
using EV.DataAccess.DataConnection;
using EV.DataAccess.SettingConfigurations;
using EV.DataAccess.UnitOfWorks;
using EV.DataAccess.UnitOfWorks.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddSingleton<JWTHelper>();

// SignalR for realtime chat or maybe realtime post, comment
builder.Services.AddSignalR();

// Add appsettings.json configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
// Map AppSettings to the EV.Common.SettingConfigurations class
builder.Services.Configure<LoginMasterKey>(builder.Configuration.GetSection("LoginMasterKey"));
builder.Services.Configure<JWTConfiguration>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("Email-Configuration"));
builder.Services.Configure<SFTPConfiguration>(builder.Configuration.GetSection("SFTP-Configuration"));
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

builder.Services.AddAuthentication
    (options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

//Service Lifetime
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();              //Register UnitOfWork
builder.Services.AddScoped<IEmailHandler, EmailHandlerService>();
builder.Services.AddScoped<IUploadFiles, UploadFilesService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

//Policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()    // Allows all origins
              .AllowAnyHeader()    // Allows all headers
              .AllowAnyMethod();   // Allows all HTTP methods
    });

    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Your frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // If using cookies or tokens
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapPost("broadcast", async (string message, IHubContext<ChatHub, IChatClient> context)=>{
//    await context.Clients.All.ReceiveMessage(message);
//    return ;
//});


app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

//Middleware Register Place
app.UseMiddleware<LoginMasterKeyCheckingMiddleware>(); //check login x-master-key middleware
app.UseWhen(                                           //check token expired (except for login)
    context => !context.Request.Path.StartsWithSegments("/api/session/login", StringComparison.OrdinalIgnoreCase),
    builder => builder.UseMiddleware<TokenExpiredCheckingMiddleware>()
    );

app.MapControllers();

app.MapHub<ChatHub>("/EnVietHub");

//app.MigrateDatabase();
app.Run();