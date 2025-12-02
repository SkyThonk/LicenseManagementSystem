namespace Common.Infrastructure.Messaging;

/// <summary>
/// Configuration options for Kafka messaging.
/// </summary>
public class KafkaSettings
{
    public const string SectionName = "Kafka";

    /// <summary>
    /// Kafka bootstrap servers (comma-separated list)
    /// </summary>
    public string BootstrapServers { get; set; } = string.Empty;

    /// <summary>
    /// Security protocol (e.g., SASL_SSL, PLAINTEXT)
    /// </summary>
    public string SecurityProtocol { get; set; } = "PLAINTEXT";

    /// <summary>
    /// SASL mechanism (e.g., PLAIN, SCRAM-SHA-256)
    /// </summary>
    public string SaslMechanism { get; set; } = string.Empty;

    /// <summary>
    /// SASL username for authentication
    /// </summary>
    public string SaslUsername { get; set; } = string.Empty;

    /// <summary>
    /// SASL password for authentication
    /// </summary>
    public string SaslPassword { get; set; } = string.Empty;

    /// <summary>
    /// Consumer group ID for this service
    /// </summary>
    public string ConsumerGroupId { get; set; } = string.Empty;

    /// <summary>
    /// Topic name for tenant events
    /// </summary>
    public string TenantEventsTopic { get; set; } = "tenant-events";
}
