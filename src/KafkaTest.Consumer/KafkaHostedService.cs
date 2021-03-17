using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaTest.Shared;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace KafkaTest.SampleApi.DataService
{
    public class KafkaHostedService : BackgroundService
    {
        private readonly string _topic;
        private readonly string _group;

        public KafkaHostedService(string topic, string group)
        {
            _topic = topic;
            _group = group;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var conf = new ConsumerConfig
            {
                GroupId = _group,
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            
            ConnectionMultiplexer muxer = await ConnectionMultiplexer.ConnectAsync(new ConfigurationOptions
            {
                EndPoints = { "host.docker.internal:6379" },
            });
            
            // var schemaRegistryConfig = new SchemaRegistryConfig
            // {
            //     Url = "localhost:8081"
            // };
            

            var redisDb = muxer.GetDatabase();
            
            using (var builder = new ConsumerBuilder<string, Person>(conf)
                .SetValueDeserializer(new Confluent.SchemaRegistry.Serdes.JsonDeserializer<Person>().AsSyncOverAsync())
                .Build())
            {
                builder.Subscribe(_topic);
                var cancelToken = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        var consumer = builder.Consume(cancelToken.Token);
                        await redisDb.StringSetAsync(consumer.Message.Key, JsonConvert.SerializeObject(consumer.Message.Value));
                    }
                }
                catch (Exception)
                {
                    builder.Close();
                }
            }
        }
    }
}