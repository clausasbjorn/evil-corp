using Newtonsoft.Json;

namespace EvilCorp.SignalHub.GoogleApi
{
    public class Request
    {
        [JsonProperty(PropertyName = "longUrl")]
        public string LongUrl { get; set; }
    }
}