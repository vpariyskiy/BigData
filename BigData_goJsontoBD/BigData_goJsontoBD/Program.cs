using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace BigData_goJsontoBD
{
    class Program
    {
       
        static void Main(string[] args)
        {
            CommonService commonService = new CommonService();
            IConnection connection = commonService.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            ReceiveSerialisationMessages(model);             
        }
        private static void ReceiveSerialisationMessages(IModel model)
        {
            model.BasicQos(0, 1, false);
            QueueingBasicConsumer consumer = new QueueingBasicConsumer(model);
            model.BasicConsume(CommonService.SerialisationQueueName, false, consumer);
            while (true)
            {
                BasicDeliverEventArgs deliveryArguments = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                String jsonified = Encoding.UTF8.GetString(deliveryArguments.Body);
                var weather = JsonConvert.DeserializeObject<WeatherJson>(jsonified);
                Console.WriteLine("Pure json: {0}", jsonified);
                //  Console.WriteLine("Скорость ветра: {0} м/с", weather.wind.speed);
                                             
                model.BasicAck(deliveryArguments.DeliveryTag, false);
                ConnectToMongoDb(jsonified);
            }
        }
        private static void ConnectToMongoDb(string jsontobson)
        {
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            server.Connect();
            var database = server.GetDatabase("BigData");
            var bsonDoc = BsonSerializer.Deserialize<BsonDocument>(jsontobson);
            var collection = database.GetCollection<BsonDocument>("bigdata_collection");

            collection.Insert(bsonDoc);
        }
        
    }
}
