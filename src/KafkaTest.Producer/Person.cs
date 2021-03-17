using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace KafkaTest.Producer
{
    public class Person
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        
        [JsonProperty("address")]
        public Address Address { get; set; }
        
    }
    
    public class Address
    {
        [JsonProperty("line1")]
        public string Line1 { get; set; }
        
        [JsonProperty("postCode")]
        public string PostCode { get; set; }
    }
    
}