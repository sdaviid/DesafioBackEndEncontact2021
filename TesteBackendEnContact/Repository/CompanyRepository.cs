using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook.Company;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Repository.Interface;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Repository;
using System;
using TesteBackendEnContact.Core;

namespace TesteBackendEnContact.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public CompanyRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task<ICompany> SaveAsync(ICompany company)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var dao = new CompanyDao(company);

            if (dao.Id == 0)
            {
                string ApiKeyUnix = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
                dao.API = Utils.CreateMD5(ApiKeyUnix);
                dao.Password = Utils.CreateMD5(dao.Password);
                //ContactBookRepository BookContact = new ContactBookRepository(databaseConfig);
                //IContactBook BookContactData = await BookContact.SaveAsync(new ContactBook(0, dao.Name));
                //dao.ContactBookId = BookContactData.Id;
                dao.Id = await connection.InsertAsync(dao);
            }
            else
                await connection.UpdateAsync(dao);

            return dao.Export();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var sql = new StringBuilder();
            sql.AppendLine("DELETE FROM Company WHERE Id = @id;");
            sql.AppendLine("UPDATE Contact SET CompanyId = null WHERE CompanyId = @id;");

            await connection.ExecuteAsync(sql.ToString(), new { id }, transaction);
            transaction.Commit();
        }

        public async Task<IEnumerable<ICompany>> GetAllAsync(string API_KEY)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = string.Format("SELECT * FROM Company WHERE API = '{0}';", API_KEY);
            var result = await connection.QueryAsync<CompanyDao>(query);

            return result?.Select(item => item.Export());
        }

        public async Task<ICompany> Login(string CNPJ, string Password)
        {
            Password = Utils.CreateMD5(Password);
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            string query = string.Format("SELECT * FROM Company WHERE CNPJ = '{0}' AND Password = '{1}';", CNPJ, Password);
            Console.WriteLine(query);
            var result = await connection.QuerySingleOrDefaultAsync<CompanyDao>(query, new { CNPJ, Password });
            return result?.Export();
        }

        public async Task<ICompany> GetAsync(int id, string API_KEY)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = string.Format("SELECT * FROM Company where Id = {0} AND API = '{1}';", id.ToString(), API_KEY);
            var result = await connection.QuerySingleOrDefaultAsync<CompanyDao>(query);

            return result?.Export();
        }
    }

    [Table("Company")]
    public class CompanyDao : ICompany
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string CNPJ {get;set;}
        public string Password {get;set;}
        public string API { get; set; }

        public CompanyDao()
        {
        }

        public CompanyDao(ICompany company)
        {
            Id = company.Id;
            Name = company.Name;
            CNPJ = company.CNPJ;
            Password = company.Password;
            API = company.API;
        }

        public ICompany Export() => new Company(Id, Name, CNPJ, Password, API);
    }
}
