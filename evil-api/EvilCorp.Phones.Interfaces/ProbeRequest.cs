using System;

namespace EvilCorp.Phones.Interfaces
{
    public class ProbeRequest
    {
        public ProbeRequest(string identifier, string ssid)
        {
            Identifier = identifier;
            Ssid = ssid;
            Timestamp = DateTime.Now;
        }

        public string Identifier { get; set; }
        public string Ssid { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
