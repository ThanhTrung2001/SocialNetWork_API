using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.RealTime;
using EnVietSocialNetWorkAPI.RealTime.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
//builder.Services.AddSingleton<ChatHub>();
builder.Services.AddSignalR();
//builder.Services.AddSingleton<Database>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/EnVietHub");

//app.MigrateDatabase();
app.Run();
