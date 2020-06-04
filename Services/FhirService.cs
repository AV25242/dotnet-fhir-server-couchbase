using FHIR.Models;
using FHIR.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace FHIR.Services
{
    [ApiController]
    [Route("[controller]")]
    public class Fhir : Controller
    {
        private readonly FhirRepository _repository;

        public Fhir(FhirRepository repository)
        {
            _repository = repository;
        }


        [HttpGet]
        [Route("api/{type}/{id?}")]
        public async Task<IActionResult> Get([FromRoute] string type, [FromRoute] string id)
        {

            dynamic result = new ExpandoObject();
            var param = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(id))
            {
                param.Add("id", id);
            }

            var querystring = HttpContext.Request.Query;
            foreach(var item in querystring)
            {
                param.Add(item.Key, item.Value);
            }

            result.result =  await _repository.GetFhir(type, param);
            return Ok(result);
        }


    }
}
