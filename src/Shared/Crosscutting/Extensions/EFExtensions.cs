using System;
using System.Collections.Generic;
using System.Linq;

namespace Stardust.Flux.Crosscutting.Extensions
{
    public static class EFExtensions
    {
        public static void MergeChildren<TSrc, TDest>(
            this ICollection<TDest> destination,
            IEnumerable<TSrc> source,
            Func<TSrc, TDest, bool> predicate,
            Func<TSrc, TDest> createCallback,
            Action<TSrc, TDest> updateCallback = null)
        {
            var itemsToRemove = destination.Where(x => source.All(src => !predicate(src, x))).ToArray();
            foreach (var toRemove in itemsToRemove)
            {
                destination.Remove(toRemove);
            }

            foreach (var itemSource in source)
            {
                var currentEntity = destination.FirstOrDefault(x => predicate(itemSource, x));
                if (currentEntity == null)
                    destination.Add(createCallback(itemSource));
                else
                    updateCallback?.Invoke(itemSource, currentEntity);
            }
        }

        public static void MergeChildren<TSrc>(
          this ICollection<TSrc> destination,
          IEnumerable<TSrc> source, Func<TSrc, TSrc, bool> predicate,
          Action<TSrc, TSrc> updateCallback = null)
        {
            var itemsToRemove = destination.Where(x => source.All(src => !predicate(src, x))).ToArray();
            foreach (var toRemove in itemsToRemove)
            {
                destination.Remove(toRemove);
            }

            foreach (var itemSource in source)
            {
                var currentEntity = destination.FirstOrDefault(x => predicate(itemSource, x));
                if (currentEntity == null)
                    destination.Add(itemSource);
                else
                    updateCallback?.Invoke(itemSource, currentEntity);
            }
        }
    }
}