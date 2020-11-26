using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models.Games
{
    public class BuyGame
    {
        public int Id { get; set; }
        public string Buyer { get; set; }
        public string GameName { get; set; }
        public string AmountPaid { get; set; }
    }
}
