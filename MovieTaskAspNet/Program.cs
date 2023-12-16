using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieTaskAspNet.Data;
using MovieTaskAspNet.Repositories.Abstract;
using MovieTaskAspNet.Repositories.Concrete;
using MovieTaskAspNet.Services.Abstract;
using MovieTaskAspNet.Services.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddSingleton<System.ComponentModel.BackgroundWorker>();

var connection = builder.Configuration.GetConnectionString("myconn");
builder.Services.AddDbContext<MovieDbContext>(opt =>
{
    opt.UseSqlServer(connection);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
