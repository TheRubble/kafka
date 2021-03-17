using System.Threading.Tasks;
using KafkaTest.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace KafkaTest.SampleApi.Controllers
{
    public class PersonController : Controller
    {
        // GET
        public async Task<IActionResult> Index(string id)
        {
            
            ConnectionMultiplexer muxer = await ConnectionMultiplexer.ConnectAsync(new ConfigurationOptions
            {
                EndPoints = { "host.docker.internal:6379" },
            });
            
            var redisDb = muxer.GetDatabase();

            var result = await redisDb.StringGetAsync(id);
            if (!string.IsNullOrEmpty(result))
            {
                return Ok(JsonConvert.DeserializeObject<Person>(result));    
            }

            return NotFound();
        }
    }
}