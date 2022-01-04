namespace TesteBackendEnContact.Core.Interface.ContactBook.Company
{
    public interface ICompany
    {
        int Id { get; }
        string Name { get; }
        string CNPJ {get;}
        string Password {get;}
        string API { get; }
    }


    public interface ICompanyList
    {
        int Id { get; }
        string Name { get; }
        string CNPJ {get;}
    }


    public interface ICompanyAdd
    {
        string Name { get; }
        string CNPJ {get;}
        string Password {get;}
    }
}
