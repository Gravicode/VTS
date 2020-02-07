using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace VTS.Receiver.Helpers
{
    public class MqttService
    {

        public delegate void OnMessageReceiverEvent(string Message);
        public event OnMessageReceiverEvent OnMessageReceived;
        public MqttService()
        {
            SetupMqtt();
        }
        MqttClient MqttClient;
       
        public void PublishMessage(string Message)
        {
            MqttClient.Publish(AppConstants.MqttTopic, Encoding.UTF8.GetBytes(Message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }
        public void SubscribeTopic(string[] Topics)
        {
            MqttClient.Subscribe(Topics, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }
            void SetupMqtt()
        {
            if (string.IsNullOrEmpty(AppConstants.MqttHost) || string.IsNullOrEmpty(AppConstants.MqttUser) || string.IsNullOrEmpty(AppConstants.MqttPass))
                throw new Exception("check your mqtt config.");
            MqttClient = new MqttClient(AppConstants.MqttHost);

            // register a callback-function (we have to implement, see below) which is called by the library when a message was received
            MqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            // use a unique id as client id, each time we start the application
            var clientId = "vts-device-receiver";//Guid.NewGuid().ToString();

            MqttClient.Connect(clientId, AppConstants.MqttUser, AppConstants.MqttPass);
            Console.WriteLine("MQTT is connected");
        } // this code runs when a message was received
        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
            OnMessageReceived?.Invoke(ReceivedMessage);
        }
        
    }
}
