using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GSM.Models
{
    public class Gadget
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string CustomerId { get; set; }
        public virtual ApplicationUser Customer { get; set; }
        public ICollection<Contract> Contracts { get; set; }
    }
}
