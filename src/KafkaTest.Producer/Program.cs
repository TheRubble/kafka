using System;
using System.IO;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;

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
            // BatchNumMessages = 10
        };
        
        static async Task Main(string[] args)
        {
            // Read using an outbox pattern.
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                try
                {

                    for (int i = 0; i < 1000000; i++)
                    {
                        var bogus = new Bogus.Person();
                        Console.WriteLine("{0} {1}\n", i, bogus.FirstName);
                        producer.Produce(SampleTopic, new Message<string, string> {Key = i.ToString(), Value = bogus.FirstName});
                    }

                    // using var file = new StreamReader("../../../../../FakeData/set1.txt");
                    //
                    // dynamic dynJson = JsonConvert.DeserializeObject(file.ReadToEnd());
                    //
                    // foreach (var item in dynJson)
                    // {
                    //     Console.WriteLine("{0} {1}\n", item.Id, item.FullName);
                    //     producer.Produce(SampleTopic, new Message<string, string> {Key = item.Id, Value = item.FullName});
                    // }
                    //
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