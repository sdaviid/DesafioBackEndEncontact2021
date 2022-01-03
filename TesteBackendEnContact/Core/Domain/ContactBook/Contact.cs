using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using System.Collections.Generic;
namespace TesteBackendEnContact.Core.Domain.Contact
{
    public class Contact : IContact
    {
        public int Id { get;  set; }
        public int ContactBookId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get;  set; }
        public string Phone { get;  set; }
        public string Email { get;  set; }
        public string Address { get;  set; }



        public Contact(int id, int contactbookid, int companyid, string name, string phone, string email, string address)
        {
            Id = id;
            ContactBookId = contactbookid;
            CompanyId = companyid;
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
        }
    }

    public class ContactAdd : IContactAdd
    {
        public int ContactBookId { get; set; }
        public string Name { get;  set; }
        public string Phone { get;  set; }
        public string Email { get;  set; }
        public string Address { get;  set; }



        public ContactAdd(int contactbookid, string name, string phone, string email, string address)
        {
            ContactBookId = contactbookid;
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
        }
    }

    public class ContactSearch : IContactSearch
    {
        public IEnumerable<IContact> Lista {get; set;}
        public int Total {get; set;}

        public ContactSearch(IEnumerable<IContact> lista, int total)
        {
            Lista = lista;
            Total = total;
        }

        // public ContactSearch(int id, int contactbookid, int companyid, string name, string phone, string email, string address, int total)
        // {
        //     Id = id;
        //     ContactBookId = contactbookid;
        //     CompanyId = companyid;
        //     Name = name;
        //     Phone = phone;
        //     Email = email;
        //     Address = address;
        //     Total = total;
        // }
    }
}