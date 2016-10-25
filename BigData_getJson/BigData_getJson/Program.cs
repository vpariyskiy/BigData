using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Reflection;
using System.Threading;

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
            for(;;)
            RunSerialisationDemo(model);
        }
    

        private static void SetupSerialisationMessageQueue(IModel model)
        {
            model.QueueDeclare(CommonService.SerialisationQueueName, false, false, false, null);
        }

        private static void RunSerialisationDemo(IModel model)
        {
            var url = "http://api.openweathermap.org/data/2.5/weather?id=" + Convert.ToString(GetIdCity()) + "&units=metric&APPID=43bef92309c42a83f24c2a114bc3fcba";
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var responseText = reader.ReadToEnd();
            var weather = responseText;

            IBasicProperties basicProperties = model.CreateBasicProperties();
            basicProperties.SetPersistent(true);
               
            byte[] customerBuffer = Encoding.UTF8.GetBytes(weather);
            model.BasicPublish("", CommonService.SerialisationQueueName, basicProperties, customerBuffer);
            Thread.Sleep(2000);

        }
        private static int GetIdCity()
        {
            StreamReader streamreader = new StreamReader("city.json");
            Random rnd = new Random();
            int r = rnd.Next(1, 209580);
            for (int i = 1; i < r; i++)
            {
                streamreader.ReadLine();
            }
            string str = streamreader.ReadLine();
            var city = JsonConvert.DeserializeObject<CityJson>(str);
            int cityid = city._id;
            streamreader.Dispose();
            return cityid;
        }
    }
}
