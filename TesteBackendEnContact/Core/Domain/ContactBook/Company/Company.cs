using TesteBackendEnContact.Core.Interface.ContactBook.Company;

namespace TesteBackendEnContact.Core.Domain.ContactBook.Company
{
    public class Company : ICompany
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public string Password { get; set; }
        public string API { get; set; }

        public Company(int id, string name, string cnpj, string password, string api)
        {
            Id = id;
            Name = name;
            CNPJ = cnpj;
            Password = password;
            API = api;
        }
    }



    public class CompanyList : ICompanyList
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string CNPJ { get; private set; }


        public CompanyList(int id, string name, string cnpj)
        {
            Id = id;
            Name = name;
            CNPJ = cnpj;
        }
    }


    public class CompanyAdd : ICompanyAdd
    {
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public string Password { get; set; }


        public CompanyAdd(string name, string cnpj, string password)
        {
            Name = name;
            CNPJ = cnpj;
            Password = password;
        }
    }

    public class CompanyUpdate : ICompanyUpdate
    {
        public string Name { get; set; }


        public CompanyUpdate(string name)
        {
            Name = name;
        }
    }
}
