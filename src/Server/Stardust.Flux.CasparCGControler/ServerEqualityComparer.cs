using System.Collections.Generic;
using Stardust.Flux.Contract;
using System.Diagnostics.CodeAnalysis;

namespace Stardust.Flux.CasparControler
{
    class ServerEqualityComparer : IEqualityComparer<ServerDto>
    {
        public bool Equals(ServerDto x, ServerDto y)
        {
            if (x == null || y == null)
                return false;

            return string.Equals(x.Hostname, y.Hostname) && x.Port == y.Port;
        }

        public int GetHashCode([DisallowNull] ServerDto obj)
        {
            unchecked
            {
                var hash = (obj.Hostname?.GetHashCode() ?? 0) * 17;
                hash += obj.Port.GetHashCode();
                return hash;
            }
        }
    }

}
