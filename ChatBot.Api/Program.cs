using ChatBot.Api.Data;
using ChatBot.Common.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChatBotDbContext>(o => o.UseSqlServer(connectionString));
builder.Services.AddScoped<HttpClient>();
builder.Services.AddSingleton(RT.Comb.Provider.Sql);
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ChatBotOrigins", builder =>
    {
        builder.WithOrigins(
           "https://localhost:7288"
        )
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials()
      .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    //migrations
    var db = scope.ServiceProvider.GetRequiredService<ChatBotDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("ChatBotOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chatHub", options =>
{
    options.AllowStatefulReconnects = true;
});

app.Run();
