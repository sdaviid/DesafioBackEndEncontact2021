using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Core.Domain.Contact;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Repository.Interface;
using System;


namespace TesteBackendEnContact.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public ContactRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }


        public async Task<dynamic> SaveAsync(dynamic contact, int IdCompany, string API_KEY)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            int Id_contact = 0;
            if(contact is ContactUpdate)
            {
                Id_contact = contact.Id;
            }
            Contact contact_data = new Contact(Id_contact, contact.ContactBookId, IdCompany, contact.Name, contact.Phone, contact.Email, contact.Address);
            var dao = new ContactDao(contact_data);

            /*
            CHECA SE ID CONTACTBOOK INFORMADO PERTENCE A EMPRESA
            */

            ContactBookRepository BookContact = new ContactBookRepository(databaseConfig);
            IContactBookDetails BookContactDetailsData = await BookContact.GetAsync(contact_data.ContactBookId, API_KEY);
            if(string.IsNullOrEmpty(Convert.ToString(BookContactDetailsData)))
                return new {error = true, error_msg = "ContactBook doesnt belong to company"};


           

            if (dao.Id == 0)
            {
                dao.Id = await connection.InsertAsync(dao);
            }
            else
            {
                 /*
                CHECA DADOS ID CONTATO
                */

                IContact ContatoData = await GetAsync(dao.Id, API_KEY);
                if(ContatoData is IContact)
                    await connection.UpdateAsync(dao);
                else
                    return new {error = true, error_msg = "ID contact doesnt belong to company"};
            }

            return dao.Export();
        }


        public async Task<dynamic> DeleteAsync(int id, string API_KEY)
        {
            IContact ContatoData = await GetAsync(id, API_KEY);
            if(ContatoData is IContact)
            {
                using var connection = new SqliteConnection(databaseConfig.ConnectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                var sql = new StringBuilder();
                sql.AppendLine("DELETE FROM Contact WHERE Id = @id;");
                

                await connection.ExecuteAsync(sql.ToString(), new { id }, transaction);
                transaction.Commit();
                return new {error = false, error_msg = ""};
            }
            else
            {
                return new {error = true, error_msg = "ID contact doesnt belong to company"};
            }
        }


        public async Task<IEnumerable<IContact>> GetAllAsync(string API_KEY, int id_contactbook=0)
        {
            
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            SqliteCommand cmd = new SqliteCommand(@"SELECT A.Id, A.ContactBookId, A.CompanyId, A.Name, 
                A.Phone, A.Email, A.Address FROM Contact A 
                LEFT JOIN Company B ON A.CompanyId = B.ID WHERE B.API = @api", connection);
            cmd.Parameters.Add(new SqliteParameter("api", API_KEY));
            string contactbooksearch = "";
            if(id_contactbook != 0)
            {
                contactbooksearch = string.Format(" AND A.ContactBookId = {0}", id_contactbook);
            }
            string query = Utils.buildQuerySqliteCmd(cmd);
            if(contactbooksearch.Count() > 0)
                query += contactbooksearch;


            var result = await connection.QueryAsync<ContactDao>(query);

            return result?.Select(item => item.Export());
        }

        public async Task<IContact> GetAsync(int id, dynamic API_KEY=null, bool PublicSearch=false)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var sql = new StringBuilder();
            string query = string.Format(@"SELECT A.Id, A.ContactBookId, A.CompanyId, {0}, {1},
                {2}, {3} FROM Contact A LEFT JOIN Company B ON A.CompanyId = B.ID WHERE A.Id = {4}", 
                (PublicSearch == false ? "A.Name" : "SUBSTR(A.Name, 1, (LENGTH(A.Name) / 2)) || '*******' AS Name"),
                (PublicSearch == false ? "A.Phone" : "SUBSTR(A.Phone, 1, (LENGTH(A.Phone) / 2)) || '*******' AS Phone"),
                (PublicSearch == false ? "A.Email" : "SUBSTR(A.Email, 1, (LENGTH(A.Email) / 2)) || '*******' AS Email"),
                (PublicSearch == false ? "A.Address" : "SUBSTR(A.Address, 1, (LENGTH(A.Address) / 2)) || '*******' AS Address"),
                id
            );
            sql.AppendLine(query);
            if((PublicSearch == false) && (!string.IsNullOrEmpty(API_KEY)))
                sql.AppendLine(string.Format("AND B.API = '{0}'", API_KEY));
            Console.WriteLine(sql.ToString());

            var result = await connection.QuerySingleOrDefaultAsync<ContactDao>(sql.ToString());

            return result?.Export();
        }

        public async Task<ContactSearch> SearchContact(dynamic API_KEY=null, dynamic ContactName=null, dynamic ContactPhone=null, dynamic ContactEmail=null, dynamic ContactAddress=null, dynamic ContactCompany=null, int ContactBookId=0, dynamic ContactBookName=null, bool PublicSearch=false, int index_start=0)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var sql = new StringBuilder();
            string query = string.Format(@"SELECT A.Id, A.ContactBookId, A.CompanyId, {0}, {1}, {2}, 
                {3} FROM COMPANY B LEFT JOIN CONTACT A ON B.ID = A.COMPANYID 
                LEFT JOIN ContactBook C ON A.ContactBookId = C.ID WHERE 1=1", 
                (PublicSearch == false ? "A.Name" : "SUBSTR(A.Name, 1, (LENGTH(A.Name) / 2)) || '*******' AS Name"),
                (PublicSearch == false ? "A.Phone" : "SUBSTR(A.Phone, 1, (LENGTH(A.Phone) / 2)) || '*******' AS Phone"),
                (PublicSearch == false ? "A.Email" : "SUBSTR(A.Email, 1, (LENGTH(A.Email) / 2)) || '*******' AS Email"),
                (PublicSearch == false ? "A.Address" : "SUBSTR(A.Address, 1, (LENGTH(A.Address) / 2)) || '*******' AS Address")
            );
            sql.AppendLine(query);
            if((PublicSearch == false) && (!string.IsNullOrEmpty(API_KEY)))
            {
                sql.AppendLine(string.Format("AND B.API = '{0}'", API_KEY.Replace("'", "\"")));
            }
            if(!String.IsNullOrEmpty(Convert.ToString(ContactName)))
            {
                sql.AppendLine(string.Format("AND A.Name LIKE '%{0}%'", ContactName.Replace("'", "\"")));
            }
            if(!string.IsNullOrEmpty(Convert.ToString(ContactPhone)))
            {
                sql.AppendLine(string.Format("AND A.Phone LIKE '%{0}%'", ContactPhone.Replace("'", "\"")));
            }
            if(!string.IsNullOrEmpty(Convert.ToString(ContactEmail)))
            {
                sql.AppendLine(string.Format("AND A.Email LIKE '%{0}%'", ContactEmail.Replace("'", "\"")));
            }
            if(!string.IsNullOrEmpty(Convert.ToString(ContactAddress)))
            {
                sql.AppendLine(string.Format("AND A.Address LIKE '%{0}%'", ContactAddress.Replace("'", "\"")));
            }
            if(!string.IsNullOrEmpty(Convert.ToString(ContactCompany)))
            {
                sql.AppendLine(string.Format("AND B.Name LIKE '%{0}%'", ContactCompany.Replace("'", "\"")));
            }
            if(ContactBookId > 0)
            {
                sql.AppendLine(string.Format("AND A.ContactBookId = {0}", ContactBookId.ToString().Replace("'", "\"")));
            }
            if(!string.IsNullOrEmpty(Convert.ToString(ContactBookName)))
            {
                sql.AppendLine(string.Format("AND C.Name LIKE '%{0}%'", ContactBookName.Replace("'", "\"")));
            }
            sql.AppendLine("AND A.Id IS NOT NULL");
            //sql.AppendLine("LIMIT 10");
            //sql.AppendLine(string.Format("OFFSET {0}", index_start));
            sql.AppendLine("ORDER BY A.Id");

            
            Console.WriteLine(sql.ToString());
            var result = await connection.QueryAsync<ContactDao>(sql.ToString());

            /* NOVO */
            var returnList = new List<IContact>();
            int linhas_max = 10;
            int linhas_atual = 0;
            if(result.ToList().Count() > 0)
            {
                foreach (var DadosContato in result.ToList())
                {
                    if(linhas_atual >= index_start)
                    {
                        if(returnList.Count() < linhas_max)
                        {
                            IContact contato = new Contact(
                                                        DadosContato.Id,
                                                        DadosContato.ContactBookId,
                                                        DadosContato.CompanyId,
                                                        DadosContato.Name.ToString(),
                                                        DadosContato.Phone.ToString(),
                                                        DadosContato.Email.ToString(),
                                                        DadosContato.Address.ToString()
                            );
                            returnList.Add(contato);
                        }
                        else
                            break;
                    }
                    linhas_atual += 1;
                }
            }
            return new ContactSearch(returnList.ToList(), result.ToList().Count());
            /*fim novo*/
        }
    }

    [Table("Contact")]
    public class ContactDao : IContact
    {
        [Key]
        public int Id { get;  set; }
        public int ContactBookId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get;  set; }
        public string Phone { get;  set; }
        public string Email { get;  set; }
        public string Address { get;  set; }



        public ContactDao()
        {
        }

        public ContactDao(IContact contact)
        {
            Id = contact.Id;
            ContactBookId = contact.ContactBookId;
            CompanyId = contact.CompanyId;
            Name = contact.Name;
            Phone = contact.Phone;
            Email = contact.Email;
            Address = contact.Address;
        }


        public IContact Export() => new Contact(Id, ContactBookId, CompanyId, Name, Phone, Email, Address);
    }


    [Table("Contact")]
    public class ContactDaoSearch : IContactSearch
    {
        [Key]

        public int Total{get; set;}
        public IEnumerable<IContact> Lista {get;set;}


        public ContactDaoSearch()
        {
        }



        public ContactDaoSearch(IEnumerable<IContact> contact, int total)
        {
            Lista = contact;
            Total = total;
        }

        public IContactSearch ExportSearch() => new ContactSearch(Lista, Total);
    }
}