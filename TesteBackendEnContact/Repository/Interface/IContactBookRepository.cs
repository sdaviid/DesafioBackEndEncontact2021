using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Interface.ContactBook;

namespace TesteBackendEnContact.Repository.Interface
{
    public interface IContactBookRepository
    {
        Task<IContactBook> SaveAsync(IContactBook contactBook, int IdCompany);
        Task<dynamic> DeleteAsync(int id, string API_KEY);
        Task<IEnumerable<IContactBookDetails>> GetAllAsync(string API_KEY);
        Task<IContactBookDetails> GetAsync(int id, string API_KEY);
    }
}
