using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebCore.Model
{
    [Table("tbl_User_Role2")]
    public class RoleUser : IdentityUserRole<string>
    {
        [Key]
        public string Id { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
        
    }
}
