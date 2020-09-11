using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebCore.Model
{
    [Table("tbl_m_user")]
    public class User : IdentityUser
    {
        public Employees Employee { get; set; }
    }
}
