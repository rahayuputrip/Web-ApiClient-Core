using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebCore.ViewModel;

namespace WebClient.Controllers
{
    public class LoginController : Controller
    {
        readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44396/api/")
        };

        //Login
        [Route("login")]
        public IActionResult Index()
        {
            return View();
        }

        //Register
        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("verify")]
        public IActionResult Verify()
        {
            return View();
        }

        [Route("notfound")]
        public IActionResult NotFound()
        {
            return View();
        }

        [Route("validate")]
        public IActionResult Validate(UserViewModel userVM)
        {
            if (userVM.UserName == null)
            {
                var jsonUserVM = JsonConvert.SerializeObject(userVM);
                var buffer = System.Text.Encoding.UTF8.GetBytes(jsonUserVM);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var resTask = client.PostAsync("user/login/", byteContent);
                resTask.Wait();
                var result = resTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = result.Content.ReadAsStringAsync().Result;
                    if (data != null)
                    {
                        HttpContext.Session.SetString("token", "Bearer " + data);                       //tambahan
                        var handler = new JwtSecurityTokenHandler();
                        var tokenS = handler.ReadJwtToken(data);

                        //var user = new UserViewModel();
                        //user.Id = tokenS.Claims.First(claim => claim.Type == "Id").Value;
                        //user.UserName = tokenS.Claims.First(claim => claim.Type == "UserName").Value;
                        //user.Email = tokenS.Claims.First(claim => claim.Type == "Email").Value;
                        //user.RoleName = tokenS.Claims.First(claim => claim.Type == "RoleName").Value;

                        var jwtPayloadSer = JsonConvert.SerializeObject(tokenS.Payload.ToDictionary(x => x.Key, x => x.Value));         //tambahan
                        var jwtPayloadDes = JsonConvert.DeserializeObject(jwtPayloadSer).ToString();
                        var user = JsonConvert.DeserializeObject<UserViewModel>(jwtPayloadSer);

                        if (user.VerifyCode != "")
                        {
                            if (userVM.VerifyCode != user.VerifyCode)
                            {
                                return Json(new { status = true, msg = "Check your Code" });
                            }
                        }
                        else if (user.RoleName == "Admin" || user.RoleName == "Sales")
                        {
                            HttpContext.Session.SetString("id", user.Id);
                            HttpContext.Session.SetString("uname", user.UserName);
                            HttpContext.Session.SetString("email", user.Email);
                            HttpContext.Session.SetString("lvl", user.RoleName);
                            if (user.RoleName == "Admin")
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "Admin" });
                            }
                            else
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "Sales" });
                            }
                        }
                        else
                        {
                            return Json(new { status = false, msg = "Invalid Username or Password!" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Username Not Found!" });
                    }
                }
                else
                {
                    //return RedirectToAction("Login","Auth");
                    return Json(new { status = false, msg = "Something Wrong!" });
                }
            }
            else if (userVM.UserName != null)
            {
                var json = JsonConvert.SerializeObject(userVM);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = client.PostAsync("user/register/", byteContent).Result;
                if (result.IsSuccessStatusCode)
                {
                    return Json(new { status = true, code = result, msg = "Register Success! " });
                }
                else
                {
                    return Json(new { status = false, msg = "Something Wrong!" });
                }
            }
            return Redirect("/login");
        }

        [Route("verifCode")]
        public IActionResult VerifCode(UserViewModel userVM)
        {
            if (userVM.VerifyCode != null)
            {
                var jsonUserVM = JsonConvert.SerializeObject(userVM);
                var buffer = System.Text.Encoding.UTF8.GetBytes(jsonUserVM);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = client.PostAsync("user/code/", byteContent).Result;
                if (result.IsSuccessStatusCode)
                {
                    var data = result.Content.ReadAsStringAsync().Result;
                    if (data != "")
                    {
                        var json = JsonConvert.DeserializeObject(data).ToString();
                        var account = JsonConvert.DeserializeObject<UserViewModel>(json);
                        if (account.RoleName == "Admin" || account.RoleName == "Sales")
                        {
                            HttpContext.Session.SetString("id", account.Id);
                            HttpContext.Session.SetString("uname", account.UserName);
                            HttpContext.Session.SetString("email", account.Email);
                            HttpContext.Session.SetString("lvl", account.RoleName);
                            if (account.RoleName == "Admin")
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "Admin" });
                            }
                            else
                            {
                                return Json(new { status = true, msg = "Login Successfully !", acc = "Sales" });
                            }
                        }
                        else
                        {
                            return Json(new { status = false, msg = "Invalid Username or Password!" });
                        }
                    }
                    else
                    {
                        return Json(new { status = false, msg = "Username Not Found!" });
                    }
                }
                else
                {
                    return Json(new { status = false, msg = "Your Code is Wrong!" });
                }
            }
            else
            {
                return Json(new { status = false, msg = "Something Wrong!" });
            }
        }


        [Route("logout")]
        public IActionResult Logout()
        {
            //HttpContext.Session.Remove("lvl");
            HttpContext.Session.Clear();
            return Redirect("/login");
        }

        [Route("getjwt")]
        public IActionResult GetName()
        {
            var stream = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6ImRiM2VhZmIxLTkyMWUtNDdmYS1hOGFiLTIwNDYxMzkxM2FlMCIsIlVzZXJuYW1lIjoiUmlmcXkiLCJFbWFpbCI6Im11aGFtbWFkcmlmcWkwQGdtYWlsLmNvbSIsIlJvbGVOYW1lIjoiU2FsZXMiLCJleHAiOjE1OTk1NDY0MTYsImlzcyI6IkludmVudG9yeUF1dGhlbnRpY2F0aW9uU2VydmVyIiwiYXVkIjoiSW52ZW50b3J5c2VydmljZVBvc3RtYW50Q2xpZW50In0.ziIjgvqJdH17w4HwHGzvXyZTUz41S06i0xHWGxAnY2M";
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadJwtToken(stream);
            //var cek = tokenS.Payload;
            //cek.u

            //var jsonToken = handler.ReadToken(stream);
            //var tokenS = handler.ReadToken(stream) as JwtSecurityToken;

            //var id = tokenS.Claims.First(claim => claim.Type == "Id").Value;
            //var uname = tokenS.Claims.First(claim => claim.Type == "Username").Value;
            //var mail = tokenS.Claims.First(claim => claim.Type == "Email").Value;
            //var role = tokenS.Claims.First(claim => claim.Type == "RoleName").Value;

            //var user = new UserViewModel()
            //{
            //    Id = tokenS.Claims.First(claim => claim.Type == "Id").Value,
            //    UserName = tokenS.Claims.First(claim => claim.Type == "Username").Value,
            //    Email = tokenS.Claims.First(claim => claim.Type == "Email").Value,
            //    RoleName = tokenS.Claims.First(claim => claim.Type == "RoleName").Value,
            //};

            //var usrVm = new UserViewModel();
            ////return Json(user);
            //return Json(tokenS.Payload);
            var jwtPayloadSer = JsonConvert.SerializeObject(tokenS.Payload.ToDictionary(x => x.Key, x => x.Value));
            var jwtPayloadDes = JsonConvert.DeserializeObject(jwtPayloadSer).ToString();
            var account = JsonConvert.DeserializeObject<UserViewModel>(jwtPayloadSer);

            // Output the whole thing to pretty Json object formatted.
            return Json(new { account.Id, account.UserName, account.Email, account.RoleName, account.VerifyCode });

        }
    }
}