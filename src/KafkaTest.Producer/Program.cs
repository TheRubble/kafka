using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaTest.AvroSchemas;
using Address = KafkaTest.Shared.Address;
using Person = KafkaTest.Shared.Person;

namespace KafkaTest.Producer
{
    class Program
    {

        private static string SampleTopic = "customer.data.v1";
        
        private  static readonly ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = "host.docker.internal:9092",
            Acks = Acks.All,
            EnableIdempotence = true,
            CompressionType = CompressionType.Snappy,
            LingerMs = 100,
        };
        
        static void Main(string[] args)
        {
            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = "localhost:8081"
            };

            var jsonSerializerConfig = new AvroSerializerConfig
            {
                BufferBytes = 100
            };

            
            var registry = new CachedSchemaRegistryClient(schemaRegistryConfig);
          
            // Read using an outbox pattern.
            using (var producer = new ProducerBuilder<string, User >(config)
                .SetValueSerializer(new AvroSerializer<User>(registry,jsonSerializerConfig).AsSyncOverAsync())
                .Build())
            {
                try
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        var bogus = new Bogus.Person();

                        var person = new User
                        {
                            favorite_color = bogus.Random.Word(),
                            favorite_number = bogus.Random.Int(0,1000),
                            hourly_rate = decimal.Round(bogus.Random.Decimal(0,100),2),
                            name = bogus.FullName
                        };
                        
                        Console.WriteLine(i);
                        producer.Produce(SampleTopic, new Message<string, User> {Key = i.ToString(), Value = person});
                    }
                    
                    producer.Flush(TimeSpan.FromSeconds(5));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Oops, something went wrong: {e}");
                }
            }
            
        }
    }
}