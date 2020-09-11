using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCore.ViewModel
{
    public class UserViewModel
    {
        public UserViewModel() { }
        public UserViewModel(UserViewModel user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            Password = user.Password;
            Phone = user.Phone;
            VerifyCode = user.VerifyCode;
            
        }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }  
        public string RoleName { get; set; }
        public string VerifyCode { get; set; }
    }
}
