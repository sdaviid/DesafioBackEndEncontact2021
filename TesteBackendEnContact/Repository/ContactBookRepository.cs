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


        public async Task<IContactBook> SaveAsync(IContactBook contactBook, int IdCompany)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var dao = new ContactBookDao(contactBook);

            if (dao.Id == 0)
            {
                dao.CompanyId = IdCompany;
                dao.Id = await connection.InsertAsync(dao);
            }
            else
                await connection.UpdateAsync(dao);

            return dao.Export();

            // dao.Id = await connection.InsertAsync(dao);

            // return dao.Export();
        }


        public async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();
            var sql = new StringBuilder();
            sql.AppendLine("DELETE FROM ContactBook WHERE Id = @id;");

            await connection.ExecuteAsync(sql.ToString(), new { id }, transaction);
            transaction.Commit();

            // TODO
            // var sql = "";

            // await connection.ExecuteAsync(sql);
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
}
