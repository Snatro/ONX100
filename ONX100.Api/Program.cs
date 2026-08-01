using ONX100.Communication;
using ONX100.Driver;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<ITcpClientConnection, TcpClientConnection>();
builder.Services.AddSingleton<DeviceMessageParser>();
builder.Services.AddSingleton<ProjectorDriver>();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("ReactPolicy");

app.MapControllers();


var driver = app.Services
    .GetRequiredService<ProjectorDriver>();
await driver.Connect();

app.Run();