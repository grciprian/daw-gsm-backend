using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GSM.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Gadget> Gadgets { get; set; }
        public ICollection<Contract> Contracts { get; set; }
    }
}
