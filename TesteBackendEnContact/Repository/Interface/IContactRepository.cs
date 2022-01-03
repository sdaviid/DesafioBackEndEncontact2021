using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Core.Domain.Contact;

namespace TesteBackendEnContact.Repository.Interface
{
    public interface IContactRepository
    {
        Task<dynamic> SaveAsync(dynamic contact, int IdCompany, string API_KEY);
        Task DeleteAsync(int id);
        Task<IEnumerable<IContact>> GetAllAsync(string API_KEY);
        Task<IContact> GetAsync(int id, dynamic API_KEY=null, bool PublicSearch=false);
        Task<ContactSearch> SearchContact(dynamic API_KEY=null, dynamic ContactName=null, dynamic ContactPhone=null, dynamic ContactEmail=null, dynamic ContactAddress=null, dynamic ContactCompany=null, int ContactBookId=0, dynamic ContactBookName=null, bool PublicSearch=false, int index_start=0);
    }
}
