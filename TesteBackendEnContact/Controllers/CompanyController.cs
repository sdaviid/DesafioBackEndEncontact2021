using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;
using TesteBackendEnContact.Repository.Interface;
using TesteBackendEnContact.Core.Domain.ContactBook.Company;

namespace TesteBackendEnContact.ControllersNonAuth
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ILogger<CompanyController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/company/register")]
        public async Task<dynamic> Post(CompanyAdd company, [FromServices] ICompanyRepository companyRepository)
        {
            dynamic resposta = await companyRepository.SaveAsync(company);
            if(resposta is ICompany)
                return resposta;
            else
                return StatusCode(StatusCodes.Status400BadRequest, resposta);
        }

        [HttpPost("/company/login")]
        public async Task<ActionResult<ICompany>> Login(string CNPJ, string Password, [FromServices] ICompanyRepository companyRepository)
        {
            var result = await companyRepository.Login(CNPJ, Password);
            return Ok(result);
        }


        [HttpGet("/company/list")]
        public async Task<IEnumerable<ICompanyList>> Get([FromServices] ICompanyRepository companyRepository)
        {
            return await companyRepository.GetAllAsync();
        }
    }
}



namespace TesteBackendEnContact.ControllersAuth
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : AuthController
    {
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ILogger<CompanyController> logger)
        {
            _logger = logger;
            KILL_NO_KEY = false;
        }


        [HttpDelete("/company/delete")]
        public async Task<dynamic> Delete(int id, [FromServices] ICompanyRepository companyRepository)
        {
            if(this.HAS_USER == true)
            {
                return await companyRepository.DeleteAsync(id, this.API_KEY, this.USER_DATA_COMPANY.Id);
            }
            return StatusCode(StatusCodes.Status401Unauthorized, new {error = true, error_msg = "Invalid API_KEY"});
        }

        // [HttpGet("/company/list")]
        // public async Task<IEnumerable<ICompanyList>> Get([FromServices] ICompanyRepository companyRepository)
        // {
        //     return await companyRepository.GetAllAsync(this.API_KEY);
        // }

        [HttpGet("/company/get/{id}")]
        public async Task<dynamic> Get(int id, [FromServices] ICompanyRepository companyRepository)
        {
            int id_company = 0;
            if(this.HAS_USER == true)
                id_company = this.USER_DATA_COMPANY.Id;
            return await companyRepository.GetAsync(id, this.API_KEY, id_company);
        }


        [HttpPost("/company/update")]
        public async Task<dynamic> Post(CompanyUpdate company, [FromServices] ICompanyRepository companyRepository)
        {
            if(this.HAS_USER == true)
            {
                dynamic resposta = await companyRepository.SaveAsync(company, this.USER_DATA_COMPANY.Id, this.API_KEY);
                if(resposta is ICompany)
                    return resposta;
                else
                    return StatusCode(StatusCodes.Status400BadRequest, resposta);
            }
            else
            {
                return StatusCode(StatusCodes.Status403Forbidden, new {error = true, error_msg = "Missing API Key"});
            }
        }
    }
}
