using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace MQTTService
{
    public class Configuration
    {
        [YamlMember(Alias = "client-id", ApplyNamingConventions = false)]
        public string ClientId { get; set; }

        public string Server { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [YamlMember(Alias = "reconnect-delay", ApplyNamingConventions = false)]
        public double ReconnectDelay { get; set; }

        public List<string> Topics { get; set; }

        [YamlMember(Alias = "use-ssl", ApplyNamingConventions = false)]
        public bool UseSSL { get; set; }
    }
}
