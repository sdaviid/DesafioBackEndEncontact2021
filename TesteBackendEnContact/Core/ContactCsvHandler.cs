using System.Collections.Generic;
using System;
using System.IO;
using TesteBackendEnContact.Core.Domain.Contact;


public class ContactModel
{
    public int ContactBookId { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    
    public ContactModel(int contactbookid, int companyid, string name, string phone, string email, string address)
    {
        ContactBookId = contactbookid;
        CompanyId = companyid;
        Name = name;
        Phone = phone;
        Email = email;
        Address = address;
    }
}
public class ContactCsvHandler
{
    public dynamic path_file { get; set; }
    //public string path_file { get; set; }
    public List<ContactAdd> list_contact = new List<ContactAdd>();

    public ContactCsvHandler(string path)
    {
        this.path_file = path;
        this.convert_csv_to_contact();
    }


    public ContactCsvHandler(System.IO.Stream path)
    {
        this.path_file = path;
        this.convert_csv_to_contact(true);
    }

    public void show_contacts()
    {
        foreach(var contato in this.list_contact)
        {
            Console.WriteLine(contato.Address);
        }
    }

    void convert_csv_to_contact(bool stream=false)
    {
        string line;
        string[] row = new string [5];
        string[] column_titles = {"ContactBookId", "Name", "Phone", "Email", "Address"};
        StreamReader sr;
        if(stream == false)
            sr = new StreamReader(this.path_file.ToString());
        else
            sr = new StreamReader(this.path_file);
        int line_count = 0;
        while ((line = sr.ReadLine()) != null)
        {
            line_count += 1;
            line = line.Replace(';', ',');
            if(line_count == 1)
            {
                if(line.Contains(column_titles[0]) == true)
                    continue;
            }
            row = line.Split(',');
            var temp = new Dictionary<string, string>();
            int column_count = 0;
            foreach(var item in row)
            {
                temp.Add(column_titles[column_count], item);
                column_count += 1;
            }
            ContactAdd res;
            if(column_count == 5)
                res = new ContactAdd(Int32.Parse(temp["ContactBookId"]), temp["Name"], temp["Phone"], temp["Email"], temp["Address"]);
            else
                res = new ContactAdd(-1, "", "", "", "");
            this.list_contact.Add(res);
        }
    } 
}