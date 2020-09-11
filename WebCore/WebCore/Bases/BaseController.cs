using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebCore.Repository.Interface;

namespace WebCore.Bases
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TEntity, TRepository> : ControllerBase
        where TEntity : class
        where TRepository : IRepository<TEntity>
    {
        private IRepository<TEntity> _repo;
        public BaseController(TRepository repository)
        {
            this._repo = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<TEntity>> GetAll() => await _repo.GetAll();

        [HttpGet("{Id}")]
        public async Task<ActionResult<TEntity>> GetById(int Id) => await _repo.GetById(Id);

        [HttpPost]
        public async Task<ActionResult<TEntity>> Post(TEntity entity)
        {
            var date = await _repo.Create(entity);
            if(date > 0)
            {
                return Ok("Data Save");
            }
            return BadRequest("Data Not Saved");
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult<int>> Delete(int Id)
        {
            var deleted = await _repo.Delete(Id);
            if (deleted.Equals(null))
            {
                return NotFound("Data not found");
            }
            return deleted;
        }


        //[HttpPut("{Id}")]
        //public async Task<ActionResult<int>> Update(TEntity entity)
        //{
        //    var update = await _repo.Update(entity);
        //    return Ok();
        //}
    }
}