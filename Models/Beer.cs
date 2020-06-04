using Newtonsoft.Json;

namespace FHIR.Models
{
    public class Beer
    {
#nullable enable
        public string? Id { get; set; }
#nullable disable
        public float Abv { get; set; }
        [JsonProperty("brewery_id")]
        public string BreweryId { get; set; }
        public float Ibu { get; set; }
        public string Name { get; set; }
        public float Srm { get; set; }
        public string Style { get; set; }
        public string Type => typeof(Beer).Name.ToLowerInvariant();
        public float Upc { get; set; }
        public string Updated { get; set; }
    }
}
