using System;
using System.Collections.Generic;
using System.Text;
using GildeApp.Api.Core.Services.Interfaces;

namespace GildeApp.Api.Core.Services.Models
{
    public class BoutScheduler : IBoutScheduler
    {
        public IReadOnlyList<(int First, int Second)> BuildOrder(int playerCount)
        {
            if (playerCount < 2)
                return Array.Empty<(int, int)>();
                       
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

                    order.Add(round % 2 == 0 ? (a, b) : (b, a));
                }

                var last = wheel[size - 1];
                wheel.RemoveAt(size - 1);
                wheel.Insert(1, last);
            }

            return order;
        }
    }
}
