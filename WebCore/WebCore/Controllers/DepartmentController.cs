using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCore.Bases;
using WebCore.Model;
using WebCore.Repository.Data;

namespace WebCore.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : BaseController<Department, DepartmentRepository>
    {
        private readonly DepartmentRepository _repo;

        public DepartmentController(DepartmentRepository departmentRepo) : base(departmentRepo)
        {
            this._repo = departmentRepo;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<int>> Update(int id, Department entity)
        {
            var getId = await _repo.GetById(id);
            getId.Name = entity.Name;
            var data = await _repo.Update(getId);
            if (data.Equals(null))
            {
                return BadRequest("Data is not Update");
            }
            return Ok("Update Successfull");
        }
    }
}