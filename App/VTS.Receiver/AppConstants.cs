using System;
using System.Collections.Generic;
using System.Text;

namespace VTS.Receiver
{
    public class AppConstants
    {
        public static string MqttHost { get; set; }
        public static string MqttUser { get; set; }
        public static string MqttPass { get; set; }
        public static string MqttTopic { get; set; } = "vts/device/data";


    }
}
