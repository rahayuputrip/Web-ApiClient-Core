using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebCore.Context;
using WebCore.Model;
using WebCore.ViewModel;

namespace WebCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyContext myContext;
        public IConfiguration _configuration;
      

        public UserController(MyContext context, IConfiguration configuration)
        {
            myContext = context;
            _configuration = configuration; 

        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        //get api values
        [HttpGet]
        //public async Task<List<User>> GetAll()
        public List<UserViewModel> GetAll()
        {
            List<UserViewModel> list = new List<UserViewModel>();
            foreach (var item in myContext.Users)
            {
                var role = myContext.RoleUsers.Where(r => r.User.Id == item.Id).FirstOrDefault();
                var role2 = myContext.Roles.Where(s => s.Id == role.RoleId).FirstOrDefault();
                UserViewModel user = new UserViewModel()
                {
                    Id = item.Id,
                    Email = item.Email,
                    UserName = item.UserName,
                    VerifyCode = item.NormalizedEmail,
                    Password = item.PasswordHash,
                    Phone = item.PhoneNumber,
                    RoleName = role2.Name
                    
                };
                list.Add(user);
            }
            return list;
            //return await _context.Users.ToListAsync<User>();
        }

        [HttpGet("{id}")]
        public UserViewModel GetID(string id)
        {
            
            var getId = myContext.Users.Find(id);
            var role = myContext.RoleUsers.Where(r => r.User.Id == getId.Id).FirstOrDefault();
            var role2 = myContext.Roles.Where(s => s.Id == role.RoleId).FirstOrDefault();
            UserViewModel user = new UserViewModel()
            {
                Id = getId.Id,
                UserName = getId.UserName,
                Email = getId.Email,
                Password = getId.PasswordHash,
                Phone = getId.PhoneNumber,
                RoleName = role2.Name
            };
            return user;
        }

        [HttpPost]
        public IActionResult Create(UserViewModel userVM)
        {
            if (ModelState.IsValid)
            {
                var random = new Random();
                var randomcode = random.Next(1, 10000).ToString("D4");

                MailMessage message = new MailMessage();
                message.From = new MailAddress("putri.baru1997@gmail.com");
                message.To.Add(userVM.Email);
                message.Subject = "Verification code";
                message.Body = "Your Verify Code is " + randomcode;

                SmtpClient smptc = new SmtpClient();
                smptc.Host = "smtp.gmail.com";
                NetworkCredential nc = new NetworkCredential();
                nc.UserName = "putri.baru1997@gmail.com";
                nc.Password = "Siskom123!";
                smptc.UseDefaultCredentials = true;
                smptc.Credentials = nc;
                smptc.Port = 587;
                smptc.EnableSsl = true;
                smptc.Send(message);

                //create akun baru
                userVM.RoleName = "Sales";
                var user = new User();
                var roleuser = new RoleUser();
                var role = myContext.Roles.Where(r => r.Name == userVM.RoleName).FirstOrDefault();
                user.UserName = userVM.UserName;
                user.Email = userVM.Email;
                user.NormalizedEmail = randomcode.ToString();
                user.EmailConfirmed = false;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userVM.Password);
                user.PhoneNumber = userVM.Phone;
                user.PhoneNumberConfirmed = false;
                user.TwoFactorEnabled = false;
                user.LockoutEnabled = false;
                user.AccessFailedCount = 0;
                roleuser.Role = role;
                roleuser.User = user;
                myContext.RoleUsers.AddAsync(roleuser);
                myContext.Users.AddAsync(user);
                myContext.SaveChanges();

                var emp = new Employees
                {
                    Id = user.Id,
                    CreateDate = DateTimeOffset.Now,
                    isDelete = false
                };
                myContext.Employees.Add(emp);
                myContext.SaveChanges();

                return Ok("Successfully Created");
            }
            return BadRequest("Register Failed");
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, UserViewModel userVM)
        {
            var getId = myContext.Users.Find(id);
            getId.Email = userVM.Email;
            getId.PasswordHash = userVM.Password;
            getId.PhoneNumber = userVM.Phone;
            var data = myContext.Users.Update(getId);
            myContext.SaveChanges();
            return Ok("Successfully Update");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (ModelState.IsValid)
            {
                var getIdr = myContext.RoleUsers.Where(g => g.UserId == id).FirstOrDefault();
                var getId = myContext.Users.Find(id);
                myContext.Users.Remove(getId);
                myContext.RoleUsers.Remove(getIdr);
                myContext.SaveChanges();
                return Ok("Successfully Delete");
            }
            return BadRequest("Account Not Found");
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(UserViewModel userVM)
        {
            if (ModelState.IsValid)
            {
                this.Create(userVM);
                return Ok("Successfully Created");
            }
            return BadRequest("Registration Failed");
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login(UserViewModel userVM)
        {
            if (ModelState.IsValid)
            {
                var userrole = myContext.RoleUsers.Include("Role").Include("User").Where(us => us.User.Email == userVM.Email).FirstOrDefault();
                if (userrole == null)
                {
                    return NotFound();
                }
                else if (userVM.Password == null || userVM.Password.Equals(""))
                {
                    return BadRequest(new { msg = "Password must filled" });
                }
                else if (!BCrypt.Net.BCrypt.Verify(userVM.Password, userrole.User.PasswordHash))
                {
                    return BadRequest(new { msg = "Password is Wrong" });
                }
                else
                {
                    ///////////////////
                    //login awalnya gini
                    //var user = new UserViewModel();
                    //user.Id = userrole.Id;
                    //user.UserName = userrole.User.UserName;
                    //user.Email = userrole.User.Email;
                    //user.VerifyCode = userrole.User.NormalizedEmail;
                    //user.RoleName = userrole.Role.Name;
                    /////////////////

                    //Untuk JWT 1
                    //            if (userrole != null)
                    //            {
                    //                if (userrole.User.SecurityStamp != null)
                    //                {
                    //                    var claims = new List<Claim> {
                    //                        new Claim("Id", userrole.User.Id),
                    //                        new Claim("UserName", userrole.User.UserName),
                    //                        new Claim("Email", userrole.User.Email),
                    //                        new Claim("RoleName", userrole.Role.Name),
                    //                        new Claim("VerifyCode", userrole.User.SecurityStamp)
                    //                    };
                    //                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    //                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    //                    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);
                    //                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    //                }
                    //                else
                    //                {
                    //                    var claims = new List<Claim> {
                    //                        new Claim("Id", userrole.User.Id),
                    //                        new Claim("UserName", userrole.User.UserName),
                    //                        new Claim("Email", userrole.User.Email),
                    //                        new Claim("RoleName", userrole.Role.Name)
                    //                    };
                    //                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    //                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    //                    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);
                    //                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    //                }
                    //            }
                    //            return BadRequest("Invalid credentials");
                    //        }
                    //    }
                    //    return BadRequest(500);
                    //}

                    //JWT2
                    if (userrole != null)
                    {
                        var claims = new List<Claim> {
                            new Claim("Id", userrole.User.Id),
                            new Claim("UserName", userrole.User.UserName),
                            new Claim("Email", userrole.User.Email),
                            new Claim("RoleName", userrole.Role.Name),
                            new Claim("VerifyCode",true ? "" : userrole.User.NormalizedEmail),
                        };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);
                        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    }
                    return BadRequest("Invalid credentials");
                }
            }
            return BadRequest(500);
        }



        [HttpPost]
        [Route("code")]
        public IActionResult VerifyCode(UserViewModel userVM)
        {
            if (ModelState.IsValid)
            {
                var getUserRole = myContext.RoleUsers.Include("User").Include("Role").SingleOrDefault(x => x.User.Email == userVM.Email);
                if (getUserRole == null)
                {
                    return NotFound();
                }
                else if (userVM.VerifyCode != getUserRole.User.NormalizedEmail)
                {
                    return BadRequest(new { msg = "Your Code is Wrong" });
                }
                else
                {

                    //            return StatusCode(200, new
                    //            {
                    //                Username = getUserRole.User.UserName,
                    //                Email = getUserRole.User.Email,
                    //                RoleName = getUserRole.Role.Name,
                    //            });
                    //        }
                    //    }
                    //    return BadRequest(500);
                    //}

                    return StatusCode(200, new
                    {
                        Username = getUserRole.User.UserName,
                        Email = getUserRole.User.Email,
                        RoleName = getUserRole.Role.Name,
                    });
                }
            }
            return BadRequest(500);
        }

    }
    
}