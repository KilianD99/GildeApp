using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Api.Core.Services.Interfaces
{
    public interface IBoutScheduler
    {
        IReadOnlyList<(int First, int Second)> BuildOrder(int playerCount);
    }
}
