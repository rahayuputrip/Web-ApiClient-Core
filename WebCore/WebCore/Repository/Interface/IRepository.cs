using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebCore.Repository.Interface
{
    public interface IRepository<T> where T : class //for every table
    {
        Task<List<T>> GetAll();
        Task<T> GetById(int Id);
        Task<int> Update(T entity);
        Task<int> Create(T entity);
        Task<int> Delete(int Id);
      //  Task<Task<ActionResult<object>>> GetById();
    }
}
