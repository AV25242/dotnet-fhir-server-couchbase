using System;
namespace FHIR.Models
{
    public interface ICouchbaserServerConfiguration
    {
        string Host { get; set; }
        string Bucket { get; set; }
        string Username { get; set; }
        string Password { get; set; }
    }

    public class CouchbaseServerConfig : ICouchbaserServerConfiguration
    {
        public CouchbaseServerConfig()
        {
        }

        public string Host { get; set; }
        public string Bucket { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
