using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace EvilCorp.Stats.Interfaces
{
    public interface IStats : IService
    {
        Task Nearby(string mac, string network);
        Task Gone(string mac);
        Task<List<KeyValuePair<string, string>>> WhoAreNearby();
    }
}
