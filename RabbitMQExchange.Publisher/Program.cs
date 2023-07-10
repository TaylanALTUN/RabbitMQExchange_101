using RabbitMQ.Client;
using RabbitMQExchange.Publisher;
using System.Text;

FanoutExchangePublisher fanoutExchange = new FanoutExchangePublisher();  
fanoutExchange.Run();


Console.ReadLine();


