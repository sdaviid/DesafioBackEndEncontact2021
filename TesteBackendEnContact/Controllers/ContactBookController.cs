using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Repository.Interface;
using Microsoft.AspNetCore.Http;

namespace TesteBackendEnContact.ControllersAuth
{
    [ApiController]
    [Route("[controller]")]
    public class ContactBookController : AuthController
    {

        private readonly ILogger<ContactBookController> _logger;

        public ContactBookController(ILogger<ContactBookController> logger)
        {
            _logger = logger;
            KILL_NO_KEY = false;
        }

        [HttpPost("/contactbook/create")]
        public async Task<dynamic> Post(ContactBookAdd contactBook, [FromServices] IContactBookRepository contactBookRepository)
        {
            if(this.HAS_USER == true)
            {
                dynamic resposta = await contactBookRepository.SaveAsync(contactBook, this.USER_DATA_COMPANY.Id, this.API_KEY);
                if(resposta is IContactBook)
                    return resposta;
                else
                    return StatusCode(StatusCodes.Status403Forbidden, resposta);
            }
            return StatusCode(StatusCodes.Status401Unauthorized, new {error = true, error_msg = "Invalid API_KEY"});
        }


        [HttpPost("/contactbook/update")]
        public async Task<dynamic> Post(ContactBookUpdate contactBook, [FromServices] IContactBookRepository contactBookRepository)
        {
            if(this.HAS_USER == true)
            {
                dynamic resposta = await contactBookRepository.SaveAsync(contactBook, this.USER_DATA_COMPANY.Id, this.API_KEY);
                if(resposta is IContactBook)
                    return resposta;
                else
                    return StatusCode(StatusCodes.Status403Forbidden, resposta);
            }
            return StatusCode(StatusCodes.Status401Unauthorized, new {error = true, error_msg = "Invalid API_KEY"});
        }


        [HttpDelete("/contactbook/delete")]
        public async Task<dynamic> Delete(int id, [FromServices] IContactBookRepository contactBookRepository)
        {
            if(this.HAS_USER == true)
            {
                return await contactBookRepository.DeleteAsync(id, this.API_KEY);
            }
            return StatusCode(StatusCodes.Status401Unauthorized, new {error = true, error_msg = "Invalid API_KEY"});
        }

        [HttpGet("/contactbook/list")]
        public async Task<IEnumerable<IContactBookDetails>> Get([FromServices] IContactBookRepository contactBookRepository)
        {
            return await contactBookRepository.GetAllAsync(this.API_KEY);
        }

        [HttpGet("/contactbook/get/{id}")]
        public async Task<dynamic> Get(int id, [FromServices] IContactBookRepository contactBookRepository)
        {
            dynamic resposta = await contactBookRepository.GetAsync(id, this.API_KEY);
            if(resposta is IContactBookDetails)
                return resposta;
            else
                return StatusCode(StatusCodes.Status401Unauthorized, new {error = true, error_msg = "No content available"});
        }
        
    }
}
