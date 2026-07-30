using ONX100.Communication;
using ONX100.Driver;

var builder = WebApplication.CreateBuilder(args);


// Controllers
builder.Services.AddControllers();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Dependency Injection
builder.Services.AddSingleton<DeviceMessageParser>();
builder.Services.AddSingleton<TcpClientConnection>();

builder.Services.AddSingleton<ProjectorDriver>();


var app = builder.Build();


// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


var driver = app.Services
    .GetRequiredService<ProjectorDriver>();
await driver.Connect();

app.Run();