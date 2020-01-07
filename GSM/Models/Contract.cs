using System;
using System.ComponentModel.DataAnnotations;

namespace GSM.Models
{
    public class Contract
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required]
        public string Observations { get; set; }
        public ContractStatus Status { get; set; }
        public string GadgetId { get; set; }
        public virtual Gadget Gadget { get; set; }
        public string EmployeeId { get; set; }
        public virtual ApplicationUser Employee { get; set; }
    }

    public enum ContractStatus
    {
        Received = 1,
        WorkInProgress = 2,
        PickUp = 3,
        Finished = 4
    }
}
