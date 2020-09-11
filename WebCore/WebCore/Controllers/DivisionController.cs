using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCore.Bases;
using WebCore.Context;
using WebCore.Model;
using WebCore.Repository.Data;

namespace WebCore.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionController : BaseController<Divisions, DivisionRepository>
    {
        private readonly DivisionRepository _repo;

        public DivisionController(DivisionRepository divisionRepo) : base(divisionRepo)
        {
            this._repo = divisionRepo;
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<int>> Update(int Id, Divisions entity)
        {
            var getId = await _repo.GetById(Id);
            getId.Name = entity.Name;
            getId.DepartmentId = entity.DepartmentId;
            var data = await _repo.Update(getId);
            if (data.Equals(null))
            {
                return BadRequest("Data is not Update");
            }
            return Ok("Update Successfull");
        }

        //[Route("GetAllDepartment")]
        //[HttpGet]
        //public async Task<IEnumerable<Divisions>> GetAllDepartment() => await _repo.GetAllDepartment();

    }

}