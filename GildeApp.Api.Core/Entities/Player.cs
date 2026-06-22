using System;
using System.Collections.Generic;
using System.Text;

namespace GildeApp.Api.Core.Entities
{
    public class Player : BaseEntity
    {
        public int FirstName { get; set; }
        public int LastName { get; set; }
        public Tourney Tourney { get; set; }
    }
}
