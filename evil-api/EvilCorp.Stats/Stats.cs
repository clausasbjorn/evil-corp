using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EvilCorp.Stats.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace EvilCorp.Stats
{
    internal sealed class Stats : StatefulService, IStats
    {
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] {
                new ServiceReplicaListener(
                    initParams => new ServiceRemotingListener<Stats>(initParams, this)
                )
            };
        }
        
        public async Task Nearby(string mac, string network)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var state = await StateManager.GetOrAddAsync<IReliableDictionary<string, KeyValuePair<string, string>>>("phones");
                
                // Update status for the phone
                await state.AddOrUpdateAsync(
                    tx, 
                    mac, 
                    new KeyValuePair<string, string>(mac, network), 
                    (_, __) => new KeyValuePair<string, string>(mac, network));

                await tx.CommitAsync();
            }
        }

        public async Task Gone(string mac)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var state = await StateManager.GetOrAddAsync<IReliableDictionary<string, KeyValuePair<string, string>>>("phones");

                // Update status for the phone
                await state.TryRemoveAsync(tx, mac);
                await tx.CommitAsync();
            }
        }

        public async Task<List<KeyValuePair<string, string>>> WhoAreNearby()
        {
            var state = await StateManager.GetOrAddAsync<IReliableDictionary<string, KeyValuePair<string, string>>>("phones");
            var nearby = state.Select(pair => pair.Value).ToList();

            return nearby;
        }
    }
}
