using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace BigData_getJson
{
    class CommonService
    {//эти строки задают начальные параметры инициализации
        private string _hostName = "localhost";
        private string _userName = "guest";
        private string _password = "guest";

        public static string SerialisationQueueName = "SerialisationDemoQueue";
        private bool _durable = false;//не сохранять сообщения на диске

        public IConnection GetRabbitMqConnection()//метод создания соединения с сервером
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = _hostName;
            connectionFactory.UserName = _userName;
            connectionFactory.Password = _password;

            return connectionFactory.CreateConnection();
        }
       }
}
