using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCore.Context;
using WebCore.Model;

namespace WebCore.Repository.Data
{
    public class DivisionRepository: GeneralRepository<Divisions, MyContext>
    {
        MyContext _context;
        public DivisionRepository(MyContext myContext) : base(myContext) //bisa pake SP dalam sini
        {
            _context = myContext;
        }

        //public async Task<List<Divisions>> GetAllDepartment()
        //{
        //    var data = await _context.Divisions.Include("Department").Where(x => x.isDelete == false).ToListAsync();
        //    return data;
        //}

        public override async Task<List<Divisions>> GetAll()
        {
            var data = await _context.Divisions.Include("Department").Where(x => x.isDelete == false).ToListAsync();
            return data;
        }

    }
}
