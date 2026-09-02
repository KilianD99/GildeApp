using System.Collections.Generic;

namespace GildeApp.Api.Core.Services.Interfaces
{
    /// <summary>
    /// Decides which positions meet, and in what order, for a tourney of a given size.
    /// </summary>
    public interface IBoutScheduler
    {
        /// <summary>
        /// Every pairing exactly once, in running order. Positions are 1-based and the
        /// returned tuples are (first, second) in the order they should be listed.
        /// </summary>
        IReadOnlyList<(int First, int Second)> BuildOrder(int playerCount);
    }
}
