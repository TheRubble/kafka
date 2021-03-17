using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace KafkaTest.SampleApi.DataService
{
    public class KafkaHostedService : IHostedService
    {
        private readonly string _topic;
        private readonly string _group;

        public KafkaHostedService(string topic, string group)
        {
            _topic = topic;
            _group = group;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var conf = new ConsumerConfig
            {
                GroupId = _group,
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            
            ConnectionMultiplexer muxer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { "host.docker.internal:6379" },
            });

            var redisDb = muxer.GetDatabase();
            
            using (var builder = new ConsumerBuilder<string, 
                string>(conf).Build())
            {
                builder.Subscribe(_topic);
                var cancelToken = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        var consumer = builder.Consume(cancelToken.Token);
                        await redisDb.StringSetAsync(consumer.Message.Key, consumer.Message.Value);
                        // Console.WriteLine($"Message: {consumer.Message.Value} received from {consumer.TopicPartitionOffset}");
                    }
                }
                catch (Exception)
                {
                    builder.Close();
                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}