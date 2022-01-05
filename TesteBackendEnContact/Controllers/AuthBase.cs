using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using TesteBackendEnContact.Database;
using Microsoft.Data.Sqlite;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using TesteBackendEnContact.Core.Domain.ContactBook.Company;



public static class authHandler
{
    public static IConfiguration Configuration;
    public static dynamic CheckAuth(HttpRequest base_req)
    {
        base_req.Query.TryGetValue("API_KEY", out var valueApiKey);
        if(string.IsNullOrEmpty(valueApiKey))
            base_req.Headers.TryGetValue("API_KEY", out valueApiKey);
        if(string.IsNullOrEmpty(valueApiKey))
            return false;
        return valueApiKey;
    }

    public static dynamic CheckAPIKey(string API_KEY)
    {
        using var connection = new SqliteConnection(authHandler.Configuration.GetConnectionString("DefaultConnection"));
        connection.Open();
        

        //PREVENIR SQL INJECTION DE FORMA CAGADA PQ TODO O RESTO TA CONCATENANDO DE FORMA RUIM E EU QUERO ENTREGAR ALGUMA COISA LOGO
        SqliteCommand cmd = new SqliteCommand("SELECT Id, Name, CNPJ, Password, API FROM COMPANY WHERE API = @api", connection);
        cmd.Parameters.Add(new SqliteParameter("api", API_KEY));
        string tmp = cmd.CommandText.ToString();
        foreach (SqliteParameter p in cmd.Parameters) {
            tmp = tmp.Replace('@' + p.ParameterName.ToString(),"'" + p.Value.ToString() + "'");
        }


        
        SqliteDataReader reader = cmd.ExecuteReader();
        bool HasRows = reader.HasRows;
        if(HasRows)
        {
            int com_id_user = 0;
            string com_name = "";
            string com_cnpj = "";
            string com_pass = "";
            string com_api = "";
            while (reader.Read())
            {
                com_id_user = reader.GetInt32(0);
                com_name = reader.GetString(1);
                com_cnpj = reader.GetString(2);
                com_pass = reader.GetString(3);
                com_api = reader.GetString(4);
            }
            connection.Close();
            Company authUserData = new Company(com_id_user, com_name, com_cnpj, com_pass, com_api);
            return authUserData;
        }
        connection.Close();
        return false;
    }
}

public class AuthController : Controller
{
    public string API_KEY = ""; 
    public Company USER_DATA_COMPANY;
    public bool KILL_NO_KEY = true;
    public bool HAS_USER = false;
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        dynamic authResult = authHandler.CheckAuth(Request);
        if(authResult != false)
        {
            dynamic IS_VALID_API_KEY = authHandler.CheckAPIKey(authResult);
            if(!(IS_VALID_API_KEY is bool))
            {
                this.API_KEY = authResult;
                this.USER_DATA_COMPANY = IS_VALID_API_KEY;
                this.HAS_USER = true;
                base.OnActionExecuting(context);
            }
            else
            {
                
                context.Result = StatusCode(StatusCodes.Status401Unauthorized, new {error = true, error_msg = "Invalid API_KEY"});
            }
        }
        else
        {
            if(this.KILL_NO_KEY == true)
                context.Result = StatusCode(StatusCodes.Status403Forbidden, new {error = true, error_msg = "Missing API_KEY"});
            else
                base.OnActionExecuting(context);
        }
    }

    
}