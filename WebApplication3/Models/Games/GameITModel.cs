using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models.Games
{
    public class GameITModel
    {
        public int Id { get; set; }
        public string GameName { get; set; }
        public string Price { get; set; }
        public string GameOwner { get; set; }
        public string GameAddressOwner {get; set;}
        public string GameAddressOwnerKey { get; set; }
    }
}
