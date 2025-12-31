// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using System.Text;
using System.Text.Json;
using NATS.Client;

class Program
{
    static void Main(string[] args)
    {
        // Connect to NATS
        Options opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = "nats://localhost:4222";

        using IConnection connection = new ConnectionFactory().CreateConnection(opts);

        Console.WriteLine("🚀 Factorial Microservice running...");
        Console.WriteLine("📌 Listening on subject: factorial.calculate");

        // Subscribe to subject
        connection.SubscribeAsync("factorial.calculate", (sender, args) =>
        {
            try
            {
                string json = Encoding.UTF8.GetString(args.Message.Data);
               var request = JsonSerializer.Deserialize<FactorialRequest>(
    json,
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    }
);


                if (request == null)
                {
                    SendError(connection, args.Message.Reply, "Invalid request payload");
                    return;
                }

                var response = FactorialHandler.Calculate(request.Number);
                SendResponse(connection, args.Message.Reply, response);
            }
            catch (Exception)
            {
                SendError(connection, args.Message.Reply, "Invalid request payload");
            }
        });

        // Keep service alive
        Console.ReadLine();
    }

    static void SendResponse(IConnection conn, string reply, FactorialResponse response)
    {
        string responseJson = JsonSerializer.Serialize(response);
        conn.Publish(reply, Encoding.UTF8.GetBytes(responseJson));
    }

    static void SendError(IConnection conn, string reply, string error)
    {
        var response = new FactorialResponse { Error = error };
        string json = JsonSerializer.Serialize(response);
        conn.Publish(reply, Encoding.UTF8.GetBytes(json));
    }
}
