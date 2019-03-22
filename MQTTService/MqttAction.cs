using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MQTTService
{
    public class MqttAction
    {
        public string Topic { get; set; }
        public string Process { get; set; }
        public string Arguments { get; set; }

        public static MqttAction GetAction(string topic)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            foreach (var file in Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "actions")))
            {
                if (Path.GetExtension(file) == ".yaml")
                {
                    var action = deserializer.Deserialize<MqttAction>(File.ReadAllText(file));
                    if (action.Topic == topic)
                        return action;
                }
            }

            return null;
        }
    }
}
