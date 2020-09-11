using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebCore.Model;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace WebClient.Controllers
{
    public class DivisionController : Controller
    {
        readonly HttpClient http = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:44396/api/")
        };

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult LoadDiv()
        {
            IEnumerable<Divisions> division = null;
            var token = HttpContext.Session.GetString("token");             //tambahan
            http.DefaultRequestHeaders.Add("Authorization", token);         //tambahan
            var resTask = http.GetAsync("division"); //controller name api
            resTask.Wait();
            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<IList<Divisions>>();
                readTask.Wait();
                division = readTask.Result;
            }
            else
            {
                division = Enumerable.Empty<Divisions>();
                ModelState.AddModelError(string.Empty, "Server Error try after sometimes.");
            }

            return Json(division);
        }

        public IActionResult GetById(int id)
        {
            Divisions division = null;
            var token = HttpContext.Session.GetString("token");             //tambahan
            http.DefaultRequestHeaders.Add("Authorization", token);         //tambahan
            var resTask = http.GetAsync("division/" + id);
            resTask.Wait();

            var result = resTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var readTask = result.Content.ReadAsAsync<Divisions>();
                readTask.Wait();

                division = readTask.Result;
            }

            return Json(division);
        }

        public JsonResult InsertOrUpdate(Divisions divisions, int id)
        {
            try
            {
                var json = JsonConvert.SerializeObject(divisions);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var token = HttpContext.Session.GetString("token");             //tambahan
                http.DefaultRequestHeaders.Add("Authorization", token);       //tambahan

                if (divisions.Id == 0)
                {
                    var result = http.PostAsync("division", byteContent).Result;
                    return Json(result);
                }
                else if (divisions.Id != 0)
                {
                    var result = http.PutAsync("division/" + id, byteContent).Result;
                    return Json(result);
                }

                return Json(404);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult Delete(int id)
        {
            var token = HttpContext.Session.GetString("token");                 //tambahan
            http.DefaultRequestHeaders.Add("Authorization", token);           //tambahan
            var result = http.DeleteAsync("division/" + id).Result;
            return Json(result);
        }
    }
}