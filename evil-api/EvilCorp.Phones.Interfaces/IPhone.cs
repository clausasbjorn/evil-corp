using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace EvilCorp.Phones.Interfaces
{
    public interface IPhone : IActor
    {
        Task Update(string network);
    }
}
