using Avro;
using KafkaTest.AvroSchemas;

namespace KafkaTest.Consumer
{
    public class UserDataTransferObject
    {
        public string Name { get; init; }
        public int? FavouriteNumber { get; init; }
        public decimal HourlyRate { get; init; }

        public static UserDataTransferObject ToDTO(User user)
        {
            return new()
            {
                Name = user.name,
                FavouriteNumber = user.favorite_number,
                HourlyRate = AvroDecimal.ToDecimal(user.hourly_rate)
            };
        }
        
    }
}