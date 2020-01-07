using GSM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GSM.Dtos
{
    public class UpdateContractDto
    {
        public string Id { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; }
    }
}
