using GSM.Models;
using System.Linq;

namespace GSM.Repositories
{
    public interface IGadgetRepository
    {
        void Create(Gadget gadget);
        int GetCount();
        Gadget GetById(string id);
        IQueryable<Gadget> GetAll();
        IQueryable<Gadget> GetAllByCustomerId(string customerId);
        bool Save();
        void Update(Gadget gadget);
    }
}