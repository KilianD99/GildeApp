using System;
using System.Collections.Generic;
using GildeApp.Api.Core.Services.Interfaces;

namespace GildeApp.Api.Core.Services
{
    /// <summary>
    /// Round robin by the circle method: everyone meets everyone exactly once, and the
    /// bouts are grouped into rounds so nobody fences twice in a row where it can be
    /// avoided.
    ///
    /// Real fencing poules use fixed published tables per poule size rather than a
    /// generated order. If you want those later, they drop straight in behind this
    /// interface without anything else changing.
    /// </summary>
    public class BoutScheduler : IBoutScheduler
    {
        public IReadOnlyList<(int First, int Second)> BuildOrder(int playerCount)
        {
            if (playerCount < 2)
                return Array.Empty<(int, int)>();

            // Odd counts get a bye that sits out one round each rotation.
            const int bye = 0;
            var wheel = new List<int>();
            for (var position = 1; position <= playerCount; position++)
                wheel.Add(position);

            if (playerCount % 2 != 0)
                wheel.Add(bye);

            var size = wheel.Count;
            var rounds = size - 1;
            var order = new List<(int, int)>();

            for (var round = 0; round < rounds; round++)
            {
                for (var i = 0; i < size / 2; i++)
                {
                    var a = wheel[i];
                    var b = wheel[size - 1 - i];

                    if (a == bye || b == bye)
                        continue;

                    // Alternate which side leads so no position is always listed first.
                    order.Add(round % 2 == 0 ? (a, b) : (b, a));
                }

                // Rotate everything except the first slot.
                var last = wheel[size - 1];
                wheel.RemoveAt(size - 1);
                wheel.Insert(1, last);
            }

            return order;
        }
    }
}
