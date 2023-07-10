using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQExchange.Publisher
{
    public class FanoutExchangePublisher
    {
        public Task Run()
        {
            var factory = new ConnectionFactory();

            //factory.Uri = new Uri("http://localhost:15672/#/");
            factory.Port = 5672;
            factory.HostName = "localhost";
            factory.UserName = "guest";
            factory.Password = "guest";

            using (var connection = factory.CreateConnection())
            {
                var channel = connection.CreateModel();

                //durable true set edildiğinde oluşan exchange kaybolmuyor.
                channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

                Enumerable.Range(1, 50).ToList().ForEach(x =>
                {
                    string message = $"log {x}";

                    var messageBody = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish("logs-fanout", "", null, messageBody);

                    Thread.Sleep(1500);

                    Console.WriteLine($"Mesaj gönderilmiştir: {message}");
                });
            }

            return Task.CompletedTask;
        }
    }
}
