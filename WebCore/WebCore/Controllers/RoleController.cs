using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCore.Context;
using WebCore.Model;

namespace WebCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly MyContext myContext;

        public RoleController(MyContext context)
        {
            myContext = context;
        }

        [HttpGet]
        public List<Role> GetAll()
        {
            List<Role> list = new List<Role>();
            foreach (var role in myContext.Roles)
            {
                list.Add(role);
            }
            return list;
        }

        [HttpPost]
        public IActionResult Create(Role role)
        {
            if (ModelState.IsValid)
            {
                if (role.Name.Equals(""))
                {
                    return BadRequest("Name Can Not Empty");
                }
                else
                {
                    var roles = new Role();
                    roles.Name = role.Name;
                    roles.NormalizedName = "false";
                    roles.ConcurrencyStamp = "false";
                    myContext.Roles.AddAsync(role);
                    myContext.SaveChanges();
                    return Ok("Role Successfully Created");
                }
            }
            return BadRequest("Role Can Not Created");
            
        }

    }
}