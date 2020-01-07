using GSM.Models;
using System;

namespace GSM.Dtos
{
    public class ContractDto
    {
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Observations { get; set; }
        public ContractStatus Status { get; set; }
        public string GadgetId { get; set; }
        public string EmployeeId { get; set; }
    }
}
