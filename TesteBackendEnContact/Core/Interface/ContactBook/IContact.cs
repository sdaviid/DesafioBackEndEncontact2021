using System.Collections.Generic;
namespace TesteBackendEnContact.Core.Interface.ContactBook.Contact
{
    public interface IContact
    {
        int Id { get; }
        int ContactBookId { get; }
        int CompanyId { get; }
        string Name { get; }
        string Phone { get; }
        string Email { get; }
        string Address { get; }
    }

    public interface IContactAdd
    {
        int ContactBookId { get; }
        string Name { get; }
        string Phone { get; }
        string Email { get; }
        string Address { get; }
    }



    public interface IContactSearch
    {
        IEnumerable<IContact> Lista {get;}
        int Total {get; }
    }


    public interface IContactUpdate
    {
        int Id { get; }
        int ContactBookId { get; }
        string Name { get; }
        string Phone { get; }
        string Email { get; }
        string Address { get; }
    }
}