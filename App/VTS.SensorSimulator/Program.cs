using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Timers;
using VTS.Parser.Messages;
using VTS.Shared;

namespace VTS.SensorSimulator
{
    class Program
    {
        static long LastIndex;
        static Random rnd;
        static List<uint> MMSIData = new List<uint>();
        static RedisDB redis;
        static MqttService service;
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                             .AddJsonFile("settings.json", true, true)
                             .AddJsonFile("local.settings.json", true, true)
                             .Build();
            AppConstants.MqttHost = config["mqtt-host"];
            AppConstants.MqttUser = config["mqtt-user"];
            AppConstants.MqttPass = config["mqtt-pass"];
            AppConstants.MqttTopic = config["mqtt-topic"];
            rnd = new Random(Environment.TickCount);
            redis = new RedisDB(config["RedisCon"], 7);
            GetAllMMSIData();
            if (service == null) service = new MqttService();
            Timer timer1 = new Timer(Convert.ToInt32(config["data-interval"]));
            timer1.Elapsed += DataSendEvent;
            timer1.Start();
            
            Console.WriteLine("simulator is started.. press any key to stop.");
            Console.ReadLine();
            timer1.Stop();

        }
        static void GetAllMMSIData()
        {
            
            var allData = redis.GetAllData<DataAIS>();
            if (allData != null && allData.Count > 0)
            {
                foreach (var item in allData)
                {
                    if (!MMSIData.Contains(item.Data.Mmsi))
                        MMSIData.Add(item.Data.Mmsi);

                    LastIndex = item.Id;
                }
            }

        }
        private static void DataSendEvent(object sender, ElapsedEventArgs e)
        {
            var obj = new DeviceData() { FlowIn=rnd.Next(1,100), FlowOut=rnd.Next(1,10), Created=DateTime.Now, SensorId="S0001", Mmsi=MMSIData[rnd.Next(0,MMSIData.Count)] };
            service.PublishMessage(JsonConvert.SerializeObject(obj));
            Console.WriteLine($"data has been sent on -> {obj.Created}");
        }

        
    }
   
}
