using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace TenantService.Infrastructure.Messaging;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, string key, string message);
}

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            SecurityProtocol = Enum.Parse<SecurityProtocol>(kafkaSettings.Value.SecurityProtocol),
            SaslMechanism = Enum.Parse<SaslMechanism>(kafkaSettings.Value.SaslMechanism),
            SaslUsername = kafkaSettings.Value.SaslUsername,
            SaslPassword = kafkaSettings.Value.SaslPassword
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
        
        _logger.LogInformation("Kafka producer initialized with bootstrap servers: {BootstrapServers}", 
            kafkaSettings.Value.BootstrapServers);
    }

    public async Task ProduceAsync(string topic, string key, string message)
    {
        try
        {
            var result = await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = message
            });

            _logger.LogInformation("Message delivered to topic {Topic}, partition {Partition}, offset {Offset}", 
                result.Topic, result.Partition, result.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to deliver message to topic {Topic}: {Error}", topic, ex.Error.Reason);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}

