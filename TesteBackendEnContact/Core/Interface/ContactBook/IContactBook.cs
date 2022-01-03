namespace TesteBackendEnContact.Core.Interface.ContactBook
{
    public interface IContactBook
    {
        int Id { get; }
        string Name { get; }
        int CompanyId {get;}
    }

    public interface IContactBookDetails
    {
        int Id { get; }
        string Name { get; }
        int CompanyId {get;}
        int TotalContacts {get;}
    }
}
