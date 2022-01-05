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

    public interface IContactBookAdd
    {
        string Name { get; }
    }

    public interface IContactBookUpdate
    {
        int Id { get; }
        string Name { get; }
    }
}
