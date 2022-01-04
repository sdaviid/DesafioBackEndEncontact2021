using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;

namespace TesteBackendEnContact.Repository.Interface
{
    public interface ICompanyRepository
    {
        Task<dynamic> SaveAsync(dynamic company);
        Task<dynamic> DeleteAsync(int id, string API_KEY, int id_company);
        Task<IEnumerable<ICompanyList>> GetAllAsync();
        Task<dynamic> GetAsync(int id, string API_KEY, int id_company=0);
        Task<ICompany> Login(string name, string senha);
    }
}
