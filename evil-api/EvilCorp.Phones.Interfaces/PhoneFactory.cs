using System;
using Microsoft.ServiceFabric.Actors;

namespace EvilCorp.Phones.Interfaces
{
    public static class PhoneFactory
    {
        private static readonly Uri PhoneServiceUrl = new Uri("fabric:/EvilCorp/PhoneActorService");

        public static IPhone CreatePhone(ActorId actorId)
        {
            return ActorProxy.Create<IPhone>(actorId, PhoneServiceUrl);
        }
    }
}
