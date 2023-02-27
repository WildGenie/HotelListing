using HotelListing.API.Data;
using HotelListing.API.Middleware;
using HotelListing.API.Configurations;
using HotelListing.API.Interfaces;
using HotelListing.API.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Json;
using Serilog;

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
/* EXTENDING JWT AUTHENTICATION TO SWAGGER DOC
--------------------------------------------*/
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swag => {
    swag.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Listing API", Version = "v1" });
    swag.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                        Enter: 'Bearer' [space] [TOKEN INPUT]",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    swag.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference =new OpenApiReference
                {
                    Type= ReferenceType.SecurityScheme,
                    Id= JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "0auth2",
                Name= JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },

            new List<string>()
        }
    });
});

/* CORS Configuration 
-------------------*/
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

/* API HEALTH CHECK SERVICES
--------------------------*/
//aspnetcore.healthchecks.sqlserver
//efcore - microsoft.extensions.diagnostics.healthchecks.entityframework
builder.Services.AddHealthChecks()
    .AddCheck<HealthCheckApi>("Health Check Api",
    failureStatus: HealthStatus.Degraded,
    tags: new[]
    {
        "api"
    })
    .AddSqlServer(connectionString, tags: new[] { "database"} )
    .AddDbContextCheck<HotelDbContext>(tags: new[] { "database" });

/* ODATA ADDED TO CONTROLLERS
---------------------------*/
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

/* HEALTH CHECK
-------------*/
app.MapHealthChecks("/healthApi", new HealthCheckOptions
{
    Predicate = healthcheck => healthcheck.Tags.Contains("api"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Degraded] = StatusCodes.Status200OK
    },

    ResponseWriter = WriteResponse
});

app.MapHealthChecks("/healthDb", new HealthCheckOptions
{
    Predicate = healthcheck => healthcheck.Tags.Contains("database"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Degraded] = StatusCodes.Status200OK
    },

    ResponseWriter = WriteResponse
});

/* CREATE JSON HEALTH REPORT STATUS 
---------------------------------*/
static Task WriteResponse(HttpContext context, HealthReport healthReport)
{
    context.Response.ContentType = "application.json; charset=utf-8";
    var options = new JsonWriterOptions { Indented = true };

    using var memoryStream = new MemoryStream();
    using (var jsonWriter = new Utf8JsonWriter(memoryStream, options)) 
    {
        jsonWriter.WriteStartObject();
        jsonWriter.WriteString("status", healthReport.Status.ToString());
        jsonWriter.WriteStartObject("result");

        foreach(var healthReportEntry in healthReport.Entries)
        {
            jsonWriter.WriteStartObject(healthReportEntry.Key);
            jsonWriter.WriteString("status",
                healthReportEntry.Value.Status.ToString());
            jsonWriter.WriteString("description",
                healthReportEntry.Value.Description);
            jsonWriter.WriteStartObject("data");

            foreach(var item in healthReportEntry.Value.Data)
            {
                jsonWriter.WritePropertyName(item.Key);

                JsonSerializer.Serialize(jsonWriter, item.Value, item.Value?.GetType() ?? typeof(object));
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        jsonWriter.WriteEndObject();
        jsonWriter.WriteEndObject();
    }

    return context.Response.WriteAsync(
        Encoding.UTF8.GetString(memoryStream.ToArray()));
}

/* REQUEST LOGGING
--------------------------------------*/
app.UseSerilogRequestLogging();

/* GLOBAL EXCEPTION HANDELING 
---------------------------*/
app.UseMiddleware<ExceptionMiddleware>(); 


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


class HealthCheckApi : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;

        if(isHealthy)
        {
            return Task.FromResult(HealthCheckResult.Healthy("SYSTEM RUNNING SMOOTH"));
        }

        return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "OEPS THERE'S AN ERROR"));
    }
}