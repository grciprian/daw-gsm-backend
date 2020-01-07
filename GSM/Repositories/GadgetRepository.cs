using GSM.Data;
using GSM.Models;
using System.Linq;

namespace GSM.Repositories
{
    public class GadgetRepository : IGadgetRepository
    {
        private readonly GSMDBContext _context;

        public GadgetRepository(GSMDBContext context)
        {
            this._context = context;
        }

        public Gadget GetById(string id)
        {
            return _context.Gadgets.FirstOrDefault(x => x.Id.Equals(id));
        }

        public int GetCount()
        {
            return _context.Gadgets.Count();
        }

        public IQueryable<Gadget> GetAll()
        {
            return _context.Gadgets;
        }

        public IQueryable<Gadget> GetAllByCustomerId(string customerId)
        {
            return _context.Gadgets.Where(x => x.CustomerId.Equals(customerId));
        }

        public void Create(Gadget gadget)
        {
            _context.Gadgets.Add(gadget);
        }

        public void Update(Gadget gadget)
        {
            _context.Gadgets.Update(gadget);
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}
