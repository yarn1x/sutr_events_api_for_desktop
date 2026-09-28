using college_events_admin_API.Models;
using college_events_admin_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseUrls("http://10.24.205.96:33679");


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true, 
            //строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,
            //будет ли валидироваться потребитель токена
            ValidateAudience = true,
            //установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            //будет ли валидироваться время существования
            ValidateLifetime = true,
            //установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            //валидация ключа безопасности
            ValidateIssuerSigningKey = true,
            //определяет по умолчанию время для валидации
            //по умолчанию выставил 0, время для валидации указано в сервисе авторизации
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