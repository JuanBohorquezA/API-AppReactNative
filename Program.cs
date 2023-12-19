using API.Middlewares;
using API.Repository;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<Encryption>();
builder.Services.AddScoped<Auth>();

var Rules = "RuleCores";
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: Rules, builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(Rules);

app.UseAuthorization();

app.MapControllers();

app.Run();
