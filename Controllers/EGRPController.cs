using WebParser.Models.EGRP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebParser.Services;

namespace WebParser.Controllers
{
    public class EGRPController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Common(IFormFile uploadedFile)
        {
            /*
            //получение списка объектов недвижимости
            List<Estate> estates = Parse(uploadedFile);
            if (estates == null) return View("Error");

            // вывод данных
            if (estates.Contains(Estate.ErrorEstate)) ViewBag.Message = "В ходе парсинга возникли ошибки! Полученная таблица содержит некорректные значения!";
            else ViewBag.Message = "Парсинг завершился успешно";

            ViewBag.Estates = estates;
            return View();
            */
            return Parse(uploadedFile, View());
        }

        [HttpPost]
        public IActionResult Inventory(IFormFile uploadedFile)
        {
            return Parse(uploadedFile, View());            
        }


        [NonAction]
        public IEGRP_Parser CheckFile(IFormFile uploadedFile)
        {
            if (uploadedFile.ContentType == "text/html") return new HTML_EGRP_Parser();
            else if (uploadedFile.ContentType == "text/xml") return new XML_EGRP_Parser();
            else return null;
        }

        [NonAction]
        public ViewResult Parse(IFormFile uploadedFile, ViewResult okViewResult)
        {
            try
            {
                IEGRP_Parser parser = CheckFile(uploadedFile);
                (List<Estate>, string) result = parser.Parse(uploadedFile);
                List<Estate> estates = result.Item1;
                ViewBag.Message = result.Item2;

                if (estates == null) return View("Error");
                if (estates.Contains(Estate.ErrorEstate)) ViewBag.Message = "В ходе парсинга возникли ошибки! Полученная таблица содержит некорректные значения!";
                ViewBag.Estates = estates;

                return okViewResult;
                //if(parser == null) return(null, "Неверный формат файла");
                //return parser.Parse(uploadedFile);

            }
            catch (Exception)
            {
                ViewBag.Message = "Непридвиденная ошибка";
                return View("Error");
            }
        }
    }
}
