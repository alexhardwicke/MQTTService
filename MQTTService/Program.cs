using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MQTTService
{
    public static class Program
    {
        public static Configuration Configuration { get; private set; }

        static async Task Main(string[] args)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            Configuration = deserializer.Deserialize<Configuration>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "configuration.yaml")));

            var mqttClientOptions = new MqttClientOptionsBuilder().WithClientId(Configuration.ClientId)
                                                                  .WithTcpServer(Configuration.Server, Configuration.Port)
                                                                  .WithCredentials(Configuration.Username, Configuration.Password);

            if (Configuration.UseSSL)
                mqttClientOptions = mqttClientOptions.WithTls();

            var options = new ManagedMqttClientOptionsBuilder().WithAutoReconnectDelay(TimeSpan.FromSeconds(Configuration.ReconnectDelay))
                                                               .WithClientOptions(mqttClientOptions.Build())
                                                               .Build();

            var client = new MqttFactory().CreateManagedMqttClient();

            client.ApplicationMessageReceived += Client_ApplicationMessageReceived;
            client.ConnectingFailed += Client_ConnectingFailed;

            foreach (var topic in Configuration.Topics)
            {
                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(topic).Build());
            }

            await client.StartAsync(options);
            Console.ReadLine();
        }

        private static void Client_ConnectingFailed(object sender, MqttManagedProcessFailedEventArgs e)
        {
            Console.WriteLine(e.Exception);
        }

        private static void Client_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            var payload = Encoding.Default.GetString(e.ApplicationMessage.Payload);

            var action = MqttAction.GetAction(payload);

            if (action != null)
            {
                Process.Start(new ProcessStartInfo(action.Process, action.Arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                });

            }
        }
    }
}
