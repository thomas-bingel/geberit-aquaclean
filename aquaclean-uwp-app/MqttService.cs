using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geberit_aquaclean_mera
{
    class MqttService
    {

        private IMqttClient client = null;

        public event EventHandler ToggleLidPosition;



        public async Task StartAsync()
        {

            // Create a new MQTT client.
            var factory = new MqttFactory();
            client = factory.CreateMqttClient();
            // Create TCP based options using the builder.
            var options = new MqttClientOptionsBuilder()
                .WithClientId("geberit")
                .WithTcpServer("192.168.0.69", 1883)
                .WithCleanSession()
                .Build();
            client.Disconnected += async (s, e) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    var result = await client.ConnectAsync(options);
                }
                catch (MQTTnet.Exceptions.MqttCommunicationException ex)
                {
                    Debug.WriteLine("Mqtt reconnect failed: " + ex.Message);
                }
            };
            client.Connected += async (s, e) =>
            {
                Debug.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic("/geberit/toggleLidPosition").Build());

                Debug.WriteLine("### SUBSCRIBED ###");
            };

            client.ApplicationMessageReceived += (s, e) =>
            {
                Debug.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Debug.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Debug.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Debug.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Debug.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Debug.WriteLine("");

                if ("/geberit/toggleLidPosition".Equals(e.ApplicationMessage.Topic)) {
                    ToggleLidPosition?.Invoke(this, null);
                }
            };

            try
            {
                var result = await client.ConnectAsync(options);
            }
            catch (MQTTnet.Exceptions.MqttCommunicationException ex)
            {
                Debug.WriteLine("### CONNECTING FAILED ###" + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("### CONNECTING FAILED ###" + ex.Message);
            }
        }

        public async Task SenDataAsync(string topic, string value)
        {
            var message = new MqttApplicationMessageBuilder()
               .WithTopic(topic)
               .WithPayload(value)
               .WithRetainFlag(true)
               .Build();
            try
            {
                await client.PublishAsync(message);
            }
            catch
            {

            }
        }

       
    }
}
