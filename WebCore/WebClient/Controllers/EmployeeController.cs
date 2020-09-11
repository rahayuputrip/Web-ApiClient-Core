using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebCore.Model;
using WebCore.ViewModel;

namespace WebClient.Controllers
{
    public class EmployeeController : Controller
    {
        readonly HttpClient http = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44396/api/")
        };

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult LoadEmp()
        {
            IEnumerable<EmployeeVM> employee = null;
            //var token = HttpContext.Session.GetString("token");             //tambahan
            //http.DefaultRequestHeaders.Add("Authorization", token);         //tambahan
            var resTask = http.GetAsync("employee"); //controller name api
            resTask.Wait();
            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<EmployeeVM>>();
                readTask.Wait();
                employee = readTask.Result;
            }
            else
            {
                employee = Enumerable.Empty<EmployeeVM>();
                ModelState.AddModelError(string.Empty, "Server Error try after sometimes.");
            }

            return Json(employee);
        }

        public IActionResult GetById(string Id)
        {
            EmployeeVM emp = null;
            //var token = HttpContext.Session.GetString("token");
            //client.DefaultRequestHeaders.Add("Authorization", token);
            var resTask = http.GetAsync("employee/" + Id);
            resTask.Wait();

            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var json = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result).ToString();
                emp = JsonConvert.DeserializeObject<EmployeeVM>(json);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Server Error.");
            }
            return Json(emp);
        }

        public IActionResult Delete(string id)
        {
            //var token = HttpContext.Session.GetString("token");
            //client.DefaultRequestHeaders.Add("Authorization", token);
            var result = http.DeleteAsync("employee/" + id).Result;
            return Json(result);
        }

    }
}