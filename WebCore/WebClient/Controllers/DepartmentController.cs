using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebCore.Model;

namespace WebClient.Controllers
{
    public class DepartmentController : Controller
    {
        readonly HttpClient http = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44396/api/")
        };

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("lvl") == "Admin")
            {
                return View();
            }
            return Redirect("/notfound");
        }

        public JsonResult LoadDepart()
        {
            IEnumerable<Department> departments = null;
            var token = HttpContext.Session.GetString("token");             //tambahan
            http.DefaultRequestHeaders.Add("Authorization", token);         //tambahan
            var resTask = http.GetAsync("department");  //departments nya ini dari si controller api
            resTask.Wait();

            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Department>>();
                readTask.Wait();
                departments = readTask.Result;

            }
            else
            {
                departments = Enumerable.Empty<Department>();
                ModelState.AddModelError(string.Empty, "Server Error try Again");
            }
            return Json(departments);
        }
        public IActionResult InsertOrUpdate(Department departments, int id_merk)
        {
            try
            {
                var json = JsonConvert.SerializeObject(departments);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var token = HttpContext.Session.GetString("token");                 //tambahan
                http.DefaultRequestHeaders.Add("Authorization", token);             //tambahan

                if (departments.Id == 0)
                {
                    var result = http.PostAsync("department", byteContent).Result;
                    return Json(result);
                }
                else if (departments.Id == departments.Id)
                {
                    var result = http.PutAsync("department/" + departments.Id, byteContent).Result;
                    return Json(result);
                }

                return Json(404);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

            public JsonResult GetById(int id)
            {
                Department department = null;
                var token = HttpContext.Session.GetString("token");             //tambahan
                http.DefaultRequestHeaders.Add("Authorization", token);         //tambahan
                var resTask = http.GetAsync("department/" + id);
                resTask.Wait();
                var result = resTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var getJson = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();
                    department = JsonConvert.DeserializeObject<Department>(getJson);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error try after sometimes.");
                }

                return Json(department);
            }

        public JsonResult Delete(int id)
        {
            var token = HttpContext.Session.GetString("token");                     //tambahan
            http.DefaultRequestHeaders.Add("Authorization", token);                 //tambahan
            var result = http.DeleteAsync("department/" + id).Result;
            return Json(result);
        }
    }
}