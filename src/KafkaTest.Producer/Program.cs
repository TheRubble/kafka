using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
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
        
        static async Task Main(string[] args)
        {
            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = "localhost:8081"
            };

            var jsonSerializerConfig = new JsonSerializerConfig
            {
                BufferBytes = 100
            };

            
            var registry = new CachedSchemaRegistryClient(schemaRegistryConfig);
          
            // Read using an outbox pattern.
            using (var producer = new ProducerBuilder<string, Person>(config)
                .SetValueSerializer(new JsonSerializer<Person>(registry,jsonSerializerConfig).AsSyncOverAsync())
                .Build())
            {
                try
                {
                    for (int i = 0; i < 1000000; i++)
                    {
                        var bogus = new Bogus.Person();

                        var person = new Person
                        {
                            FirstName = bogus.FirstName,
                            LastName = bogus.LastName,
                            Address = new Address
                            {
                                Line1 = bogus.Address.Street,
                                PostCode = bogus.Address.ZipCode
                            }
                        };
                        
                        Console.WriteLine(i);
                        producer.Produce(SampleTopic, new Message<string, Person> {Key = i.ToString(), Value = person});
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