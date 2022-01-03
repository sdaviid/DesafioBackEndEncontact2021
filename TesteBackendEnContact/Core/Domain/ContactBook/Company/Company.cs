using TesteBackendEnContact.Core.Interface.ContactBook.Company;

namespace TesteBackendEnContact.Core.Domain.ContactBook.Company
{
    public class Company : ICompany
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string CNPJ { get; private set; }
        public string Password { get; private set; }
        public string API { get; private set; }

        public Company(int id, string name, string cnpj, string password, string api)
        {
            Id = id;
            Name = name;
            CNPJ = cnpj;
            Password = password;
            API = api;
        }
    }
}
