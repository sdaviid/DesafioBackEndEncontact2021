using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Repository.Interface;
using System;

namespace TesteBackendEnContact.Repository
{
    public class ContactBookRepository : IContactBookRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public ContactBookRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }


        public async Task<dynamic> SaveAsync(dynamic contactBook, int IdCompany, string API_KEY)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            int id_contactbook = 0;
            if(contactBook is ContactBookUpdate)
            {
                id_contactbook = contactBook.Id;
            }
            ContactBook contactbook_data = new ContactBook(id_contactbook, contactBook.Name, IdCompany);
            var dao = new ContactBookDao(contactbook_data);

            if (dao.Id == 0)
            {
                dao.Id = await connection.InsertAsync(dao);
            }
            else
            {
                //CHECA SE ID PERTENCE A COMPANY
                dynamic ContactBookDetalhes = await GetAsync(dao.Id, API_KEY);
                if(ContactBookDetalhes is IContactBookDetails)
                    await connection.UpdateAsync(dao);
                else
                    return new {error = true, error_msg = "ID contactBook doesnt belong to company"};
            }

            return dao.Export();

        }


        public async Task<dynamic> DeleteAsync(int id, string API_KEY)
        {
            dynamic Contatobookdetalhes = await GetAsync(id, API_KEY);
            if(Contatobookdetalhes is IContactBookDetails)
            {
                using var connection = new SqliteConnection(databaseConfig.ConnectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();
                var sql = new StringBuilder();
                sql.AppendLine("DELETE FROM ContactBook WHERE Id = @id;");
                sql.AppendLine("DELETE FROM Contact WHERE ContactBookId = @id;");

                await connection.ExecuteAsync(sql.ToString(), new { id }, transaction);
                transaction.Commit();
                return new {error = false, error_msg = ""};
            }
            else
            {
                return new {error = true, error_msg = "ID contactBook doesnt belong to company"};
            }
        }




        public async Task<IEnumerable<IContactBookDetails>> GetAllAsync(string API_KEY)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var sql = new StringBuilder();
            string query = string.Format("SELECT A.Id, {0}, A.CompanyId, COUNT(C.Id) As TotalContacts FROM ContactBook A LEFT JOIN Company B ON A.CompanyID = B.Id LEFT JOIN Contact C ON A.Id = C.ContactBookId WHERE 1=1", 
                (string.IsNullOrEmpty(API_KEY) ? "SUBSTR(A.Name, 1, (LENGTH(A.Name) / 2)) || '*******' AS Name" : "A.Name")
            );
            sql.AppendLine(query);
            if(!string.IsNullOrEmpty(API_KEY))
                sql.AppendLine(string.Format("AND B.API = '{0}'", API_KEY));
            sql.AppendLine("GROUP BY A.Id");
            Console.WriteLine(sql.ToString());
            var result = await connection.QueryAsync<ContactBookDaoList>(sql.ToString());

            var returnList = new List<IContactBookDetails>();

            foreach (var AgendaSalva in result.ToList())
            {
                IContactBookDetails Agenda = new ContactBookDetails(AgendaSalva.Id, AgendaSalva.Name.ToString(), AgendaSalva.CompanyId, AgendaSalva.TotalContacts);
                returnList.Add(Agenda);
            }

            return returnList.ToList();
        }
        public async Task<IContactBookDetails> GetAsync(int id, string API_KEY)
        {
            var list = await GetAllAsync(API_KEY);

            return list.ToList().Where(item => item.Id == id).FirstOrDefault();
        }
    }

    [Table("ContactBook")]
    public class ContactBookDao : IContactBook
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId {get; set;}

        public ContactBookDao()
        {
        }

        public ContactBookDao(IContactBook contactBook)
        {
            Id = contactBook.Id;
            Name = contactBook.Name;
            CompanyId = contactBook.CompanyId;
        }

        public IContactBook Export() => new ContactBook(Id, Name, CompanyId);
    }


    [Table("ContactBook")]
    public class ContactBookDaoList : IContactBookDetails
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyId {get; set;}
        public int TotalContacts {get;set;}

        public ContactBookDaoList()
        {
        }

        public ContactBookDaoList(IContactBookDetails contactBookDetails)
        {
            Id = contactBookDetails.Id;
            Name = contactBookDetails.Name;
            CompanyId = contactBookDetails.CompanyId;
            TotalContacts = contactBookDetails.TotalContacts;
        }

        public IContactBookDetails Export() => new ContactBookDetails(Id, Name, CompanyId, TotalContacts);
    }


    [Table("ContactBook")]
    public class ContactBookDaoAdd : IContactBookAdd
    {
        public string Name { get; set; }

        public ContactBookDaoAdd()
        {
        }

        public ContactBookDaoAdd(IContactBookAdd contactBookAdd)
        {
            Name = contactBookAdd.Name;
        }

        public IContactBookAdd Export() => new ContactBookAdd(Name);
    }


    [Table("ContactBook")]
    public class ContactBookDaoUpdate : IContactBookUpdate
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ContactBookDaoUpdate()
        {
        }

        public ContactBookDaoUpdate(IContactBookUpdate contactBookup)
        {
            Id = contactBookup.Id;
            Name = contactBookup.Name;
        }

        public IContactBookUpdate Export() => new ContactBookUpdate(Id, Name);
    }
}
