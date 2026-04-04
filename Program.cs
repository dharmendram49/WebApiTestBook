using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using WebApiTestBook.Data;
using WebApiTestBook.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



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
if (app.Environment.IsDevelopment())
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
