using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCore.Context;
using WebCore.Model;

namespace WebCore.Repository.Data
{
    public class DepartmentRepository : GeneralRepository<Department, MyContext>
    {
        public DepartmentRepository(MyContext myContext) : base(myContext) //bisa pake SP dalam sini
        {
           
        }
    }
}
