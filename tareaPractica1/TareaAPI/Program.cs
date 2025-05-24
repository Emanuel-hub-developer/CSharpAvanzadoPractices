using Microsoft.EntityFrameworkCore;
using TareaAPI.Infrastructure.Data;
using TareaAPI.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<TareaAPIContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("API_Tarea"));
});


builder.Services.AddSingleton<TaskQueueManager>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var taskQueueManager = app.Services.GetRequiredService<TaskQueueManager>();

taskQueueManager.TaskEvents.Subscribe(mensaje =>
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"[EVENTO Reactivo] {mensaje}");
    Console.ResetColor();

});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
