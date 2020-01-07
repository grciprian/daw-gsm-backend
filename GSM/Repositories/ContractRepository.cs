using GSM.Data;
using GSM.Models;
using System.Linq;

namespace GSM.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly GSMDBContext _context;

        public ContractRepository(GSMDBContext context)
        {
            this._context = context;
        }

        public Contract GetById(string id)
        {
            return _context.Contracts.FirstOrDefault(x => x.Id.Equals(id));
        }

        public int GetCount()
        {
            return _context.Contracts.Count();
        }

        public IQueryable<Contract> GetAll()
        {
            return _context.Contracts;
        }

        public IQueryable<Contract> GetAllByGadgetId(string gadgetId)
        {
            return _context.Contracts.Where(x => x.GadgetId.Equals(gadgetId));
        }

        public IQueryable<Contract> GetAllByEmployeeId(string employeeId)
        {
            return _context.Contracts.Where(x => x.EmployeeId.Equals(employeeId));
        }

        public void Create(Contract contract)
        {
            _context.Contracts.Add(contract);
        }

        public void Update(Contract contract)
        {
            _context.Contracts.Update(contract);
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}
