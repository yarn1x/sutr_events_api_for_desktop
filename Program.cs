using college_events_admin_API.Models;
using college_events_admin_API.Models.Data_transfer_objects;
using college_events_admin_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseUrls("http://192.168.1.253:33679");

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
builder.Services.Configure<JwtOptions>(jwtSection);
var jwtOptions = jwtSection.Get<JwtOptions>()
    ?? throw new InvalidOperationException("Jwt settings are missing in configuration");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateLifetime = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Secret)),

            ValidateIssuerSigningKey = true,

            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<HashService>();

builder.Services.AddScoped<EventsService>();
builder.Services.AddScoped<OrganizerService>();
builder.Services.AddScoped<SupervisorService>();
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddHostedService<BackgroundUpdateService>();
builder.Services.AddDbContext<SutrEventsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        )
);

//builder.Services.AddW3CLogging(logging =>
//{
//    //logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.W3CLoggingFields.ClientIpAddress
//    //                      | Microsoft.AspNetCore.HttpLogging.W3CLoggingFields.Method
//    //                      | Microsoft.AspNetCore.HttpLogging.W3CLoggingFields.UriQuery
//    //                      | Microsoft.AspNetCore.HttpLogging.W3CLoggingFields.Date
//    //                      | Microsoft.AspNetCore.HttpLogging.W3CLoggingFields.Time;
//    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.W3CLoggingFields.All;
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseW3CLogging();
//app.UseHttpsRedirection();

app.UseAuthentication(); //сначала аутентификация ВАЖЕН ПОРЯДОК
app.UseAuthorization(); //потом авторизация

app.MapControllers();

//app.Run("http://0.0.0.0:33679");
app.Run();