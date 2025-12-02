namespace TenantService.Infrastructure.Messaging;

public class KafkaSettings
{
    public const string SectionName = "Kafka";
    
    public string BootstrapServers { get; set; } = string.Empty;
    public string SecurityProtocol { get; set; } = string.Empty;
    public string SaslMechanism { get; set; } = string.Empty;
    public string SaslUsername { get; set; } = string.Empty;
    public string SaslPassword { get; set; } = string.Empty;
}

