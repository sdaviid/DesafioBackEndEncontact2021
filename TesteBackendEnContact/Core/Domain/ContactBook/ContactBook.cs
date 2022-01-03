    using TesteBackendEnContact.Core.Interface.ContactBook;

namespace TesteBackendEnContact.Core.Domain.ContactBook
{
    public class ContactBook : IContactBook
    {
        public int Id { get;  set; }
        public string Name { get;  set; }
        public int CompanyId{get;set;}

        public ContactBook(int id, string name, int companyid)
        {
            Id = id;
            Name = name;
            CompanyId = companyid;
        }
    }

    public class ContactBookDetails : IContactBookDetails
    {
        public int Id { get;  set; }
        public string Name { get;  set; }
        public int CompanyId{get;set;}
        public int TotalContacts {get;set;}

        public ContactBookDetails(int id, string name, int companyid, int total_contacts)
        {
            Id = id;
            Name = name;
            CompanyId = companyid;
            TotalContacts = total_contacts;
        }
    }

}
