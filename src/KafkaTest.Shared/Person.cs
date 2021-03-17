using System;
using Newtonsoft.Json;

namespace KafkaTest.Shared
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
}