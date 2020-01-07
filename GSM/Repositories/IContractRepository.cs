using GSM.Models;
using System.Linq;

namespace GSM.Repositories
{
    public interface IContractRepository
    {
        void Create(Contract contract);
        int GetCount();
        Contract GetById(string id);
        IQueryable<Contract> GetAll();
        IQueryable<Contract> GetAllByEmployeeId(string employeeId);
        IQueryable<Contract> GetAllByGadgetId(string gadgetId);
        bool Save();
        void Update(Contract contract);
    }
}