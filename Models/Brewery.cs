using System.Collections.Generic;

namespace FHIR.Models
{
    public class Brewery
    {
        public string Id { get; set; }
        public List<string> Address { get; set; }
        public string City { get; set; }
        public string Code { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public dynamic Geo { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string State { get; set; }
        public string Type => typeof(Brewery).Name.ToLowerInvariant();
        public string Updated { get; set; }
        public string Website { get; set; }
    }

    public class Geo
    {
        public string Accuracy { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }
    }
}
