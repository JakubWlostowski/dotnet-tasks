using demo.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.IO;

namespace demo.Controllers
{
    [Route("/validation")]
    public class ValidationController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View(new ExampleForm());
        }

        [Route("submit")]
        [HttpPost]
        public IActionResult Submit(ExampleForm form)
        {
            if (!ModelState.IsValid)
            {
                return View(viewName: "Index", form);
            }

            if (form.FilesList != null)
            {
                var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "SavedFiles");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                foreach (var file in form.FilesList)
                {
                    Task.Run(async () => {                 
                        var path = Path.Combine(dirPath, file.FileName);
                        System.Console.WriteLine($"Wielkość pliku {file.Length} | Nazwa pliku {file.FileName}");
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    });
                }
            } 

            return RedirectToAction(nameof(Index));
        }

        [Route("self-form")]
        public IActionResult SelfValidatableFormAction()
        {
            return View(new SelfValidatableForm());
        }

        [HttpPost]
        [Route("self-form")]
        public IActionResult SelfValidatableFormAction(SelfValidatableForm form)
        {
            if (!ModelState.IsValid)
            {
                return View("SelfValidatableFormAction", form);
            }

            return RedirectToAction(nameof(SelfValidatableFormAction));
        }

        [AcceptVerbs("GET", "POST")]
        [Route("check-country")]
        public IActionResult CheckCountry(string country)
        {
            if (country != "Polska" && country != "Niemcy")
            {
                return Json("Niepoprawna nazwa kraju");
                
            }

            return Json(true);
        }

        [Route("check-user-exists")]
        public IActionResult UserExists(string Name, string Surname)
        {
            if (Name == "Jan" && Surname == "Kowalski")
            {
                return Json("Użytkownik o podanym imieniu i nazwisku już istnieje");
            }

            return Json(true);
        }
    }
}