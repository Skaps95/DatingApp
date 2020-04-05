using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    //Authorization details in startup.cs
    [Authorize] //If we add this annotation then all requests in this controllers can be made only by authorized sources. 
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly DataContext _context;
        public ValuesController(DataContext context)
        {
            _context = context;
        }

        // GET api/values
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            // throw new Exception("This is to check if developer exception page is showing up or not.");
            // return new string[] { "value1", "value2" };

            var values = await _context.Values.ToListAsync();
            return Ok(values);

        }


        // GET api/values/5
        [AllowAnonymous] //This will let a record with particular Id be fetched by bypassing the authorization rule (anyone can access)
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            var value = await _context.Values.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}