using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;

namespace speech_to_text_winform.services
{
    internal class MqttService
    {

        private string bucketName = "speech-to-text-resources-bucket";

        // used to pass messages back to UI for processing
        public delegate void OnMessagesReceived(string messages);
        public event OnMessagesReceived OnConnectionChanges;
        public event OnMessagesReceived OnMessagesArrive;

        private readonly Config _config;
        public MqttFactory factory;
        public IMqttClient mqttClient;
        public IMqttClientOptions options;

        public Form1 form;


        public MqttService(Form1 form)
        {
            _config = new Config();
            this.form = form;
        }

        public async Task ConnectBrokerAsync()
        {
            // Create a new MQTT client.
            factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();
            // Create TCP based options using the builder.
            options = new MqttClientOptionsBuilder()
                .WithTcpServer(_config.BROKER_URL, _config.BROKER_PORT)
                .WithCredentials(_config.BROKER_USERNAME, _config.BROKER_PASSWORD)
                .WithCleanSession()
                .Build();

            mqttClient.UseConnectedHandler(MqttConnectedHandlerAsync);

            mqttClient.UseDisconnectedHandler(MqttDisconnectedHandler);

            mqttClient.UseApplicationMessageReceivedHandler(MqttOnMessagesReceived);

            await mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken
        }

        private async Task MqttDisconnectedHandler(MqttClientDisconnectedEventArgs e)
        {
            Console.WriteLine("### DISCONNECTED FROM SERVER ###");

            OnConnectionChanges("DISCONNECTED");

            await Task.Delay(TimeSpan.FromSeconds(5));

            try
            {
                await mqttClient.ReconnectAsync();
            }
            catch
            {
                Console.WriteLine("### RECONNECTING FAILED ###");
                OnConnectionChanges("RECONNECTING");
            }
        }

        private async Task MqttConnectedHandlerAsync(MqttClientConnectedEventArgs e)
        {
            Console.WriteLine("### CONNECTED WITH SERVER ###");

            OnConnectionChanges("CONNECTED");

            await SingleSubscribeTopic("transcribe_audio");
        }

        private async void MqttOnMessagesReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            Console.WriteLine();

            var topic = e.ApplicationMessage.Topic;
            var msg = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            OnMessagesArrive($@"Topic = {topic}" + Environment.NewLine + $"Messages = {msg}");

            if (topic.Equals("transcribe_audio"))
            {
                form.DownloadVideo(msg);
            }
        }

        public async Task SendMessages(string topic, string msg)
        {
            try
            {
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(msg)
                    .WithAtMostOnceQoS()
                    .Build();

                await mqttClient.PublishAsync(message);
            }
            catch (Exception x)
            {
                Console.WriteLine($@"[SendMessages] an error has occured {x.Message}");
            }
        }

        public async Task SingleSubscribeTopic(string topic)
        {
            try
            {
                await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic($"{topic}").Build());
                Console.WriteLine(@"[SingleSubscribeTopic] Subscribed");
            }
            catch (Exception x)
            {
                Console.WriteLine($@"[SingleSubscribeTopic] an error has occured {x.Message}");
            }
        }
    }
}