using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guardian.Infrastructure.Security.Specs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Guardian.Domain.Target;
using Guardian.Domain;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Guardian.Web.UI.Controllers.Api
{
    [Route("api/[controller]")]
    public class TargetsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IIdentityHelper _identityHelper;

        public TargetsController(IMediator mediator, IIdentityHelper identityHelper)
        {
            _mediator = mediator;
            _identityHelper = identityHelper;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<QueryListResult<TargetDto>> List()
        {
            //TODO: Add paging.

            return await _mediator.Send(new List.Query());
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
