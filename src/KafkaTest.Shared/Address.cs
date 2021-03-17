using Newtonsoft.Json;

namespace KafkaTest.Shared
{
    public class Address
    {
        [JsonProperty("line1")]
        public string Line1 { get; set; }
        
        [JsonProperty("postCode")]
        public string PostCode { get; set; }
    }
}