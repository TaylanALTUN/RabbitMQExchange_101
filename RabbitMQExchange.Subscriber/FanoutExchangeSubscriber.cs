using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQExchange.Subscriber
{
    public  class FanoutExchangeSubscriber
    {
        public void Run()
        {
            var factory = new ConnectionFactory();

            //factory.Uri = new Uri("http://localhost:15672/#/");
            factory.Port = 5672;
            factory.HostName = "localhost";
            factory.UserName = "guest";
            factory.Password = "guest";

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            var randomQueueName = channel.QueueDeclare().QueueName;

            //kuyruğu kalıcı hale getirir.randomQueueName değişkeni sabit verilmeli.
            channel.QueueDeclare(randomQueueName,true,false,false);

            channel.QueueBind(randomQueueName, "logs-fanout", "", null);

            //RabbitMQ den mesajları kaçar kaçar alınacağı belirtiliyor.
            //prefetchSize: Gönderilen mesajın boyutu. 0 set edilirse herhangi boyutta mesaj gönderliebilir.
            //prefetchCount: Tek seferde kaç mesaj geleceği belirtiliyor.
            //global: false set edilirse her bir subscribera prefetchCount da belirtilen değer kadar mesaj gönderilir. true set edilirse prefetchCount da belirlenen adet paylaştırılır.
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            //autoAck true set edilirse rabbitmq subscribera mesaj gönderdiğinde mesaj doğru işlenip işlenmediğine bakmadan kuyruktan siler
            channel.BasicConsume(randomQueueName, false, consumer);

            Console.WriteLine("Loglar dinleniyor...");

           consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500);

                Console.WriteLine($"Gelen Mesaj: {message}");

                //Mesajın silinebileceği bildiriliyor.
                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}
