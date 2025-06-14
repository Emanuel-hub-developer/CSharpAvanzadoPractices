
//Primero instalamos el paquete SignalR.Client para poder probar


//Colocamos el endpoint en el cual esta concentrado y establecido para realizar la accion.
using Microsoft.AspNetCore.SignalR.Client;

string url = "https://localhost:7010/receiveNotificationTask"; 


var connection = new HubConnectionBuilder()
    .WithUrl(url)
    .Build();


connection.On<string>("receiveNotificationTask1",(msj) => { 
    Console.WriteLine($"Notificacion Recibida: {msj}");
});

try
{
    await connection.StartAsync();

    Console.WriteLine("Conectando all hub...");


} catch(Exception ex)
{
    Console.WriteLine($"Ha sucedido un error: {ex.Message}");
}

Console.ReadLine();