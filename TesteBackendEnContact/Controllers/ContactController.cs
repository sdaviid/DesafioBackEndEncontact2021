
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Core.Domain.Contact;
using TesteBackendEnContact.Repository.Interface;
using System;
using System.IO;
using TesteBackendEnContact.Core;
using TesteBackendEnContact.Controllers;

namespace TesteBackendEnContact.ControllersNonAuth
{
    // [ApiController]
    [Route("[controller]")]
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;

        public ContactController(ILogger<ContactController> logger)
        {
            _logger = logger;
        }
        [HttpGet("/seeHeader2")]
        public ActionResult GetHeader()
        {
            Request.Headers.TryGetValue("auth", out var valueHeader);
            if(string.IsNullOrEmpty(valueHeader))
                return StatusCode(StatusCodes.Status403Forbidden, "asdasdad");
            else
                return Ok(valueHeader);
        }


        [HttpGet("/Contact/public/search")]
        public async Task<IContactSearch> PublicSearch([FromServices] IContactRepository contactRepository, string ContactName="", string ContactPhone="", string ContactEmail="", string ContactAddress="", string CompanyName="", int AgendaId=0, string AgendaName="", int start_from=0)
        {

            return await contactRepository.SearchContact(ContactName:ContactName, ContactPhone:ContactPhone, ContactEmail:ContactEmail, ContactAddress:ContactAddress, ContactCompany:CompanyName, ContactBookId:AgendaId, ContactBookName:AgendaName, PublicSearch:true, index_start:start_from);
        }


        [HttpGet("/Contact/public/get/{id}")]
        public async Task<IContact> Get(int id, [FromServices] IContactRepository contactRepository)
        {
            return await contactRepository.GetAsync(id, PublicSearch:true);
        }
    }
}



namespace TesteBackendEnContact.ControllersAuth
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : AuthController
    {
        private readonly ILogger<ContactController> _logger;

        public ContactController(ILogger<ContactController> logger)
        {
            _logger = logger;
        }
        
        
        
        [HttpPost("/Contact/update")]
        public async Task<dynamic> Post(Contact contact, [FromServices] IContactRepository contactRepository)
        {
            return await contactRepository.SaveAsync(contact, this.USER_DATA_COMPANY.Id, this.API_KEY);
        }

        [HttpPost("/Contact/add")]
        public async Task<dynamic> Post(ContactAdd contact, [FromServices] IContactRepository contactRepository)
        {
            dynamic resposta = await contactRepository.SaveAsync(contact, this.USER_DATA_COMPANY.Id, this.API_KEY);
            if(resposta is IContact)
                return resposta;
            else
            {
                return StatusCode(StatusCodes.Status403Forbidden, resposta);
            }
        }

        [HttpDelete]
        public async Task Delete(int id, [FromServices] IContactRepository contactRepository)
        {
            await contactRepository.DeleteAsync(id);
        }



        [HttpGet("/Contact/list")]
        public async Task<IEnumerable<IContact>> Get([FromServices] IContactRepository contactRepository)
        {
            Console.WriteLine(this.API_KEY);
            Console.WriteLine(this.USER_DATA_COMPANY.Name);
            return await contactRepository.GetAllAsync(this.API_KEY);
        }


        [HttpGet("/Contact/get/{id}")]
        public async Task<IContact> Get(int id, [FromServices] IContactRepository contactRepository)
        {
            return await contactRepository.GetAsync(id, this.API_KEY);
        }


        [HttpGet("/Contact/search")]
        public async Task<IContactSearch> Search([FromServices] IContactRepository contactRepository, string ContactName="", string ContactPhone="", string ContactEmail="", string ContactAddress="", string CompanyName="", int AgendaId=0, string AgendaName="", int start_from=0)
        {
            return await contactRepository.SearchContact(API_KEY:this.API_KEY, ContactName:ContactName, ContactPhone:ContactPhone, ContactEmail:ContactEmail, ContactAddress:ContactAddress, ContactCompany:CompanyName, ContactBookId:AgendaId, ContactBookName:AgendaName, index_start:start_from);
        }


        [HttpPost("/Contact/import")]
        public async Task<dynamic> EnviaArquivo([FromForm] IFormFile arquivo, [FromServices] IContactRepository contactRepository)
        {
            List<dynamic> status = new List<dynamic>();
            ContactCsvHandler c = new ContactCsvHandler(arquivo.OpenReadStream());
            foreach(var contato in c.list_contact)
            {
                object teste = new {};
                if(contato.ContactBookId == -1)
                {
                    teste = new {status = "Invalid format contact"};
                }
                else
                {
                    dynamic resposta = await contactRepository.SaveAsync(contato, this.USER_DATA_COMPANY.Id, this.API_KEY);
                    
                    if(resposta is IContact)
                    {
                        teste = new {contactBookId = contato.ContactBookId, companyId = this.USER_DATA_COMPANY.Id, name = contato.Name, ContactId = resposta.Id, Address = contato.Address, Phone = contato.Phone, Email = contato.Email, status = "OK"};
                    }
                    else
                    {
                        teste = new {contactBookId = contato.ContactBookId, companyId = this.USER_DATA_COMPANY.Id, name = contato.Name, status = resposta};
                    }
                }
                status.Add(teste);
            }
            c.show_contacts();
            return status;
            //return await Task.FromResult("aushuhas".ToString());
        }

    }
}
