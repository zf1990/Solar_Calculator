using System;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace API.Controllers
{
    public class UserConotroller : BaseApiController
    {
        private readonly DataContext _context;
        public UserConotroller(DataContext context)
        {
            this._context = context;

        }

        [HttpGet]
        [Route("{ID}")]
        public async Task<ActionResult<User>> GetUser(Guid ID)
        {
            return await _context.Users.FindAsync(ID);
        }
    }
}