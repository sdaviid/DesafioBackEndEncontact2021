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
//using TesteBackendEnContact.Core;

namespace TesteBackendEnContact.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public CompanyRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task<dynamic> SaveAsync(dynamic company, int IdCompany=0, string API_KEY="")
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            int id_company = 0;
            if(company is CompanyUpdate)
            {
                id_company = IdCompany;
                if(string.IsNullOrEmpty(API_KEY))
                    return new {error = true, error_msg = "Need API Key from company for update"};
                dynamic company_data = await GetAsync(id_company, API_KEY, id_company);
                if(company_data is ICompany)
                {
                    var daoUpdate = new CompanyDao(new Company(id_company, company.Name, company_data.CNPJ, company_data.Password, company_data.API));
                    await connection.UpdateAsync(daoUpdate);
                    return daoUpdate.Export();
                }
                return new {error = true, error_msg = "ID company doesnt belong to company"};
                //var daoUpdate = new CompanyDaoUpdate(company);
                //await connection.UpdateAsync(daoUpdate);
                
            }
            else
            {
                var dao = new CompanyDao(new Company(0, company.Name, company.CNPJ, company.Password, ""));


                var empresas = await GetAllAsync();
                foreach(var i in empresas.ToList())
                {
                    ICompanyList t = new CompanyList(i.Id, i.Name, i.CNPJ);
                    if(t.CNPJ == dao.CNPJ)
                    {
                        return new {error = true, error_msg = "CNPJ already in use"};
                    }
                }
                string ApiKeyUnix = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(); //PEGAR TIMEUNIX PRA GERAR MD5
                dao.API = Utils.CreateMD5(ApiKeyUnix); //GERAR MD5 PRA KEY DA API
                dao.Password = Utils.CreateMD5(dao.Password); //CONVERTER SENHA PRA MD5 (NAO UMA BOA PRATICA, MAS PRA SISTEMA SIMPLES...)
                
                dao.Id = await connection.InsertAsync(dao);

                return dao.Export();
            }
        }

        public async Task<dynamic> DeleteAsync(int id, string API_KEY, int id_company)
        {
            dynamic CompanyData = await GetAsync(id, API_KEY, id_company);
            if(CompanyData is ICompany)
            {
                using var connection = new SqliteConnection(databaseConfig.ConnectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                var sql = new StringBuilder();
                sql.AppendLine("DELETE FROM Company WHERE Id = @id;");
                sql.AppendLine("DELETE FROM Contact WHERE CompanyId = @id;");
                sql.AppendLine("DELETE FROM ContactBook WHERE CompanyId = @id;");

                await connection.ExecuteAsync(sql.ToString(), new { id }, transaction);
                transaction.Commit();
                return new {error = false, error_msg = ""};
            }
            else
            {
                return new {error = true, error_msg = "ID company doesnt belong to company"};
            }
        }

        public async Task<IEnumerable<ICompanyList>> GetAllAsync()
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = string.Format("SELECT Id, Name, CNPJ FROM Company;");
            var result = await connection.QueryAsync<CompanyDaoList>(query);

            return result?.Select(item => item.Export());
        }

        public async Task<ICompany> Login(string CNPJ, string Password)
        {
            Password = Utils.CreateMD5(Password);
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var sql = new StringBuilder();
            //PREVENIR SQL INJECTION NO LOGIN...
            sql.AppendLine("SELECT * FROM Company WHERE CNPJ = @CNPJ AND Password = @Password;");

            //string query = string.Format("SELECT * FROM Company WHERE CNPJ = '{0}' AND Password = '{1}';", CNPJ, Password);
            //Console.WriteLine(query);
            //var result = await connection.QuerySingleOrDefaultAsync<CompanyDao>(query, new { CNPJ, Password });
            var result = await connection.QuerySingleOrDefaultAsync<CompanyDao>(sql.ToString(), new { CNPJ, Password });
            return result?.Export();
        }

        public async Task<dynamic> GetAsync(int id, string API_KEY, int id_company=0)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            bool has_full_data = false;
            var sql = new StringBuilder();
            var query = "SELECT Id, Name, CNPJ";
            sql.AppendLine(query);
            var query_end = new StringBuilder();
            query_end.AppendLine(string.Format("FROM Company WHERE ID = {0}", id));
            if((!string.IsNullOrEmpty(API_KEY)) && (id_company == id))
            {
                
                has_full_data = true;
                sql.AppendLine(", Password, API");
                query_end.AppendLine(string.Format("AND API = '{0}'", API_KEY));
            }
            sql.AppendLine(query_end.ToString());
            dynamic result;
            Console.WriteLine(sql.ToString());
            if(has_full_data == true)
                result = await connection.QuerySingleOrDefaultAsync<CompanyDao>(sql.ToString());
            else
                result = await connection.QuerySingleOrDefaultAsync<CompanyDaoList>(sql.ToString());
            //var query = string.Format("SELECT Id, Name, CNPJ FROM Company where Id = {0} AND API = '{1}';", id.ToString(), API_KEY);
            //var result = await connection.QuerySingleOrDefaultAsync<CompanyDao>(query);

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


    [Table("Company")]
    public class CompanyDaoList : ICompanyList
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string CNPJ {get;set;}

        public CompanyDaoList()
        {
        }

        public CompanyDaoList(ICompanyList company)
        {
            Id = company.Id;
            Name = company.Name;
            CNPJ = company.CNPJ;
        }

        public ICompanyList Export() => new CompanyList(Id, Name, CNPJ);
    }


    [Table("Company")]
    public class CompanyDaoAdd : ICompanyAdd
    {
        public string Name { get; set; }
        public string CNPJ {get;set;}
        public string Password {get;set;}

        public CompanyDaoAdd()
        {
        }

        public CompanyDaoAdd(ICompanyAdd company)
        {
            Name = company.Name;
            CNPJ = company.CNPJ;
            Password = company.Password;
        }

        public ICompanyAdd Export() => new CompanyAdd(Name, CNPJ, Password);
    }

    [Table("Company")]
    public class CompanyDaoUpdate : ICompanyUpdate
    {
        public string Name { get; set; }

        public CompanyDaoUpdate()
        {
        }

        public CompanyDaoUpdate(ICompanyUpdate company)
        {
            Name = company.Name;
        }

        public ICompanyUpdate Export() => new CompanyUpdate(Name);
    }
}
