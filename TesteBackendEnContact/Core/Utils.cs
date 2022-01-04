using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;


public class Utils
{
    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    public static string buildQuerySqliteCmd(SqliteCommand cmd)
    {
        string tmp = cmd.CommandText.ToString();
        Console.WriteLine(tmp);
        foreach (SqliteParameter p in cmd.Parameters) {
            tmp = tmp.Replace('@' + p.ParameterName.ToString(),"'" + p.Value.ToString() + "'");
        }
        Console.WriteLine(tmp);
        return tmp;
    }
}