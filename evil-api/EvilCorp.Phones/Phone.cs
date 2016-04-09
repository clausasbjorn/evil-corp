using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using EvilCorp.Phones.Interfaces;
using EvilCorp.Stats.Interfaces;

namespace EvilCorp.Phones
{
    internal class Phone : StatefulActor<Phone.ActorState>, IPhone
    {
        private IActorTimer _timer;

        [DataContract]
        internal sealed class ActorState
        {
            [DataMember]
            public string MacAddress { get; set; }

            [DataMember]
            public bool IsNearby { get; set; }

            [DataMember]
            public DateTime? LastUpdated { get; set; }

            [DataMember]
            public List<string> Networks { get; set; }
        }
        
        protected override Task OnActivateAsync()
        {
            var actorId = this.GetActorId().GetStringId();

            if (State == null)
                State = new ActorState { MacAddress = actorId, LastUpdated = null, Networks = new List<string>() };
            
            _timer = RegisterTimer(o => CheckNearbyStatus((ActorState)o), State, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(60));

            return Task.FromResult(true);
        }

        private Task CheckNearbyStatus(ActorState state)
        {
            if (state.IsNearby 
                && state.LastUpdated.HasValue 
                && DateTime.Now.Subtract(state.LastUpdated.Value).TotalSeconds > 60)
            {
                state.IsNearby = false;
                StatsFactory.CreateStats().Gone(State.MacAddress);
            }

            return Task.FromResult(true);
        }

        private void UpdateNetwork(string network)
        {
            if (!State.Networks.Contains(network))
                State.Networks.Add(network);
        }

        private void UpdateNearby(string network)
        {
            State.IsNearby = true;
            StatsFactory.CreateStats().Nearby(State.MacAddress, network);
        }
        
        public Task Update(string network)
        {
            State.LastUpdated = DateTime.Now;

            UpdateNetwork(network);
            UpdateNearby(network);

            return Task.FromResult(true);
        }
    }
}
