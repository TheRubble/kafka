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
            BatchSize = 10
            
        };
        
        static async Task Main(string[] args)
        {
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                try
                {
                    
                    using var file = new StreamReader("../../../../../FakeData/set1.txt");
                    
                    dynamic dynJson = JsonConvert.DeserializeObject(file.ReadToEnd());

                    foreach (var item in dynJson)
                    {
                        Console.WriteLine("{0} {1}\n", item.Id, item.FullName);
                        producer.Produce(SampleTopic, new Message<string, string> {Key = Guid.NewGuid().ToString(), Value = item.FullName});
                    }
                    
                    await Task.Delay(5000);
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