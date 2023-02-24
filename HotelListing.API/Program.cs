using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using HotelListing.API.Configurations;
using HotelListing.API.Interfaces;
using HotelListing.API.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HotelListing.API.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);

/* Connection with HotelAPIDb 
------------------------------------*/
var connectionString = builder.Configuration.GetConnectionString("HotelAPIDbString");
builder.Services.AddDbContext<HotelDbContext>(options => {
    options.UseSqlServer(connectionString);
});

/* CONFIGURING IDENTITY CORE
--------------------------*/
builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<User>>("HotelListing")
    .AddEntityFrameworkStores<HotelDbContext>().AddDefaultTokenProviders();


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

/* IMPLEMENTING API VERSIONING
----------------------------*/
builder.Services.AddApiVersioning(version =>
{
    version.AssumeDefaultVersionWhenUnspecified = true;
    version.DefaultApiVersion = new ApiVersion(1, 0);
    version.ReportApiVersions = true;
    version.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("X-version"),
        new MediaTypeApiVersionReader("ver")
        );
});

builder.Services.AddVersionedApiExplorer(exploder =>
{
    exploder.GroupNameFormat = "'v'VVV";
    exploder.SubstituteApiVersionInUrl = true;
});

/* LOGGING SERILOG & SEQ 
-----------------------*/
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

/*Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();*/

//Serilog.Debugging.SelfLog.Enable(Console.Error); // IF IN USE SCAFFOLD ERROR!!!

/* AUTO MAPPER CONFIGURATION
--------------------------*/
builder.Services.AddAutoMapper(typeof(MapperConfig));

builder.Services.AddScoped(typeof(MapperConfig));
builder.Services.AddScoped<ICountriesRepo, CountryRepo>();
builder.Services.AddScoped<IHotelsRepo, HotelRepo>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();

/* JWT AUTHENTICATION TOKEN
-------------------------*/
builder.Services.AddAuthentication(jwt =>
{
    jwt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    jwt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwt =>
    {
        jwt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
        };
    });

/* RESPONSE CACHING
-----------------*/
builder.Services.AddResponseCaching(cach =>
{
    cach.MaximumBodySize = 1024;
    cach.UseCaseSensitivePaths = true;
});

// Add services to the container.
builder.Services.AddControllers().AddOData(data =>
{
    data.Select().Filter().OrderBy();
});
//.AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/* GLOBAL EXCEPTION HANDELING 
---------------------------*/
app.UseMiddleware<ExceptionMiddleware>(); 

/* REQUEST LOGGING
--------------------------------------*/
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

/* CACHING (ADD BEHIND CORS)
--------*/
app.UseResponseCaching();
app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl =
    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
    {
        Public = true,
        MaxAge = TimeSpan.FromSeconds(10)
    };

    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
    new string[] { "Accept-Encoding" };

    await next();
});

/* ADDING AUTHENTICATION
----------------------*/
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
