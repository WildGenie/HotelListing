using HotelListing.API.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

/* Connection with HotelAPIDb 
------------------------------------*/
var connectionString = builder.Configuration.GetConnectionString("HotelAPIDbString");
builder.Services.AddDbContext<HotelDbContext>(options => {
    options.UseSqlServer(connectionString);
});

// Add services to the container.
builder.Services.AddControllers();

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
--------------------------------------*/
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

/*Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();*/

Serilog.Debugging.SelfLog.Enable(Console.Error);


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
