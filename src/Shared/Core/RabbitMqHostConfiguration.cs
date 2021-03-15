namespace Stardust.Flux.Core.Configuration
{
    public class RabbitMqHostConfiguration
    {
        public string Hostname { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; } = "/";

    }
}