using college_events_admin_API.Models;
using college_events_admin_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

//кому не повезло с умением чтени€ легаси,
//пиши на @tgn0sense или открывай обсуждение в репозитории
//https://github.com/yarn1x/sutr_events_api_for_desktop



var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseUrls("http://10.24.205.96:33679");


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // указывает, будет ли валидироватьс€ издатель при валидации токена
            ValidIssuer = AuthOptions.ISSUER,// строка, представл€юща€ издател€
            ValidateAudience = true,// будет ли валидироватьс€ потребитель токена
            ValidAudience = AuthOptions.AUDIENCE,// установка потребител€ токена
            ValidateLifetime = true,// будет ли валидироватьс€ врем€ существовани€
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),// установка ключа безопасности
            ValidateIssuerSigningKey = true,// валидаци€ ключа безопасности
            ClockSkew = TimeSpan.Zero,
        };
    });
builder.Services.AddAuthorization();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<EventsService>();
builder.Services.AddScoped<OrganizerService>();
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddHostedService<BackgroundUpdateService>();
builder.Services.AddDbContext<SutrEventsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
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
app.UseAuthentication(); //сначала аутентификаци€ ¬ј∆≈Ќ ѕќ–яƒќ 
app.UseAuthorization(); //потом авторизаци€
app.MapControllers();

//app.Run("http://0.0.0.0:33679");
app.Run();