using System;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace EvilCorp.Stats.Interfaces
{
    public static class StatsFactory
    {
        private static readonly Uri StatsServiceUrl = new Uri("fabric:/EvilCorp/Stats");

        public static IStats CreateStats()
        {
            return ServiceProxy.Create<IStats>(0, StatsServiceUrl);
        }
    }
}
