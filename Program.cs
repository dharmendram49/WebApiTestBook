using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Text;
using WebApiTestBook.Data;
using WebApiTestBook.Middlewares;
using WebApiTestBook.Models;
using WebApiTestBook.Services.Interfaces;
using WebApiTestBook.Services;
using WebApiTestBook.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "WebApiTestBook_";
});

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<IEmailService, MailinatorEmailService>();
//builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<ICacheService, InMemoryCacheService>();
builder.Services.AddScoped<ExecutionTimeFilter>();
builder.Services.AddScoped<ResponseWrapperFilter>();
builder.Services.AddScoped<MVCExceptionFilter>();
builder.Services.AddScoped<IMasterService, MasterService>();
builder.Services.AddScoped<IOtpService, OtpService>();

builder.Services.AddScoped<IPayment, UpiPayment>();
builder.Services.AddScoped<IPayment, CardPayment>();

builder.Services.AddScoped<CacheResourceFilter>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("Admin");
    });
    options.AddPolicy("ManagerOnly", policy =>
    {
        policy.RequireRole("Manager");
    });
    options.AddPolicy("AdminAndManagerOnly", policy =>
    {
        policy.RequireRole("Admin","Manager");
    });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
    };
});

//Configure serilog

// Column configuration (optional but recommended)
var columnOptions = new ColumnOptions();

// Remove Properties XML column (optional)
columnOptions.Store.Remove(StandardColumn.Properties);

// Add JSON column instead
columnOptions.Store.Add(StandardColumn.LogEvent);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true
        },
        columnOptions: columnOptions
    )
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); // show detailed error
}
else
{
    app.UseExceptionHandler("/error"); // must create error end point
}

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
//    db.Database.Migrate();
//}


app.UseMiddleware<RequestLoggingMiddleware>();   // 🔥 First
app.UseMiddleware<ExceptionMiddleware>();        // 🔥 Second
app.UseMiddleware<UserContextMiddleware>();  

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
