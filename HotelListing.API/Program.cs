using HotelListing.API.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics;
using HotelListing.API.Configurations;
using System.Text.Json.Serialization;
using HotelListing.API.Interfaces;
using HotelListing.API.Repository;

var builder = WebApplication.CreateBuilder(args);

/* Connection with HotelAPIDb 
------------------------------------*/
var connectionString = builder.Configuration.GetConnectionString("HotelAPIDbString");
builder.Services.AddDbContext<HotelDbContext>(options => {
    options.UseSqlServer(connectionString);
});

// Add services to the container.
builder.Services.AddControllers();
    //.AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* CORS Configuration 
-------------------------------------*/
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
        policy => policy
        .AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowAnyMethod());
});

/* LOGGING SERILOG & SEQ 
-----------------------*/
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

/*Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();*/

//Serilog.Debugging.SelfLog.Enable(Console.Error);

/* AUTO MAPPER CONFIGURATION
------------------------------------*/
builder.Services.AddAutoMapper(typeof(MapperConfig));

builder.Services.AddScoped(typeof(MapperConfig));
builder.Services.AddScoped<ICountriesRepo, CountryRepo>();
builder.Services.AddScoped<IHotelsRepo, HotelRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/* REQUEST LOGGING
--------------------------------------*/
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
