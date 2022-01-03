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

        [HttpPost]
        public async Task<IContactBook> Post(ContactBook contactBook, [FromServices] IContactBookRepository contactBookRepository)
        {
            if(this.HAS_USER == true)
                return await contactBookRepository.SaveAsync(contactBook, this.USER_DATA_COMPANY.Id);
            return null;
        }

        [HttpDelete]
        public async Task Delete(int id, [FromServices] IContactBookRepository contactBookRepository)
        {
            await contactBookRepository.DeleteAsync(id);
        }

        [HttpGet]
        public async Task<IEnumerable<IContactBookDetails>> Get([FromServices] IContactBookRepository contactBookRepository)
        {
            return await contactBookRepository.GetAllAsync(this.API_KEY);
        }

        [HttpGet("{id}")]
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
