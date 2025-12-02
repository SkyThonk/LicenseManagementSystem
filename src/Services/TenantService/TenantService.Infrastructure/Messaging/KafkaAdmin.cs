using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TenantService.Infrastructure.Messaging;

public interface IKafkaAdmin
{
    Task CreateTopicIfNotExistsAsync(string topicName, int numPartitions = 1, short replicationFactor = 3);
}

public class KafkaAdmin : IKafkaAdmin
{
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaAdmin> _logger;

    public KafkaAdmin(IOptions<KafkaSettings> settings, ILogger<KafkaAdmin> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task CreateTopicIfNotExistsAsync(string topicName, int numPartitions = 1, short replicationFactor = 3)
    {
        var config = new AdminClientConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            SecurityProtocol = Enum.Parse<SecurityProtocol>(_settings.SecurityProtocol),
            SaslMechanism = Enum.Parse<SaslMechanism>(_settings.SaslMechanism),
            SaslUsername = _settings.SaslUsername,
            SaslPassword = _settings.SaslPassword
        };

        using var adminClient = new AdminClientBuilder(config).Build();

        try
        {
            // Check if topic exists
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            var topicExists = metadata.Topics.Any(t => t.Topic == topicName);

            if (!topicExists)
            {
                _logger.LogInformation("Creating topic: {TopicName}", topicName);
                
                var topicSpec = new TopicSpecification
                {
                    Name = topicName,
                    NumPartitions = numPartitions,
                    ReplicationFactor = replicationFactor
                };

                await adminClient.CreateTopicsAsync(new[] { topicSpec });
                _logger.LogInformation("Topic created successfully: {TopicName}", topicName);
            }
            else
            {
                _logger.LogInformation("Topic already exists: {TopicName}", topicName);
            }
        }
        catch (CreateTopicsException ex)
        {
            _logger.LogError(ex, "Failed to create topic: {TopicName}", topicName);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during topic creation: {TopicName}", topicName);
            throw;
        }
    }
}

