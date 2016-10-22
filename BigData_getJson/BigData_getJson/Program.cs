using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace BigData_getJson
{

    class Program
    {
        static void Main(string[] args)
        {
            CommonService commonService = new CommonService();
            IConnection connection = commonService.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            SetupSerialisationMessageQueue(model);
            RunSerialisationDemo(model);
        }
    

    private static void SetupSerialisationMessageQueue(IModel model)
        {
            model.QueueDeclare(CommonService.SerialisationQueueName, false, false, false, null);
        }

        private static void RunSerialisationDemo(IModel model)
        {
            var url = "http://api.openweathermap.org/data/2.5/weather?q=Volgograd,ru&units=metric&APPID=43bef92309c42a83f24c2a114bc3fcba";
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var responseText = reader.ReadToEnd();
            var weather = responseText;

            IBasicProperties basicProperties = model.CreateBasicProperties();
            basicProperties.SetPersistent(true);
               
            byte[] customerBuffer = Encoding.UTF8.GetBytes(weather);
            model.BasicPublish("", CommonService.SerialisationQueueName, basicProperties, customerBuffer);
            
        }
    }
}
