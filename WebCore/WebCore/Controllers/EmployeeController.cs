using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCore.Context;
using WebCore.ViewModel;

namespace WebCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        public EmployeeController(MyContext myContext)
        {
            _context = myContext;
        }
        private readonly MyContext _context;
        // GET api/values

        //[Route("GetAllEmployee")]
        [HttpGet]
        public async Task<List<EmployeeVM>> GetAll()
        {
            List<EmployeeVM> list = new List<EmployeeVM>();
            //var user = new UserVM();
            var getData = await _context.Employees.Include("User").Where(x => x.isDelete == false).ToListAsync();
            if (getData.Count == 0)
            {
                return null;
            }
            foreach (var employee in getData)
            {
                var emp = new EmployeeVM()
                {
                    Id = employee.User.Id,
                    UserName = employee.User.UserName,
                    Address = employee.Address,
                    Phone = employee.User.PhoneNumber,
                    CreateDate = employee.CreateDate,
                    UpdateDate = employee.UpdateDate
                };
                list.Add(emp);
            }
            return list;
        }

        //[Authorize]
        [HttpGet("{id}")]
        public EmployeeVM GetById(string id)
        {
            var getData = _context.Employees.Include("User").SingleOrDefault(x => x.Id == id);
            if (getData == null || getData.User == null)
            {
                return null;
            }
            var emp = new EmployeeVM()
            {
                Id = getData.User.Id,
                UserName = getData.User.UserName,
                Address = getData.Address,
                Phone = getData.User.PhoneNumber,
                CreateDate = getData.CreateDate,
                UpdateDate = getData.UpdateDate
            };
            return emp;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (ModelState.IsValid)
            {
                var getData = _context.Employees.Include("User").SingleOrDefault(x => x.Id == id);
                if (getData == null)
                {
                    return BadRequest("Not Successfully");
                }
                getData.DeleteDate = DateTimeOffset.Now;
                getData.isDelete = true;

                //_context.Employees.Update(getData);
                _context.Entry(getData).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok("Successfully Deleted");
            }
            return BadRequest("Not Successfully");
        }
    }
}
