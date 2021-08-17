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
            //получение списка объектов недвижимости
            List<Estate> estates = Parse(uploadedFile);
            if (estates == null) return View("Error");

            // вывод данных
            if (estates.Contains(Estate.ErrorEstate)) ViewBag.Message = "В ходе парсинга возникли ошибки! Полученная таблица содержит некорректные значения!";
            else ViewBag.Message = "Парсинг завершился успешно";

            ViewBag.Estates = estates;
            return View();
        }

        [HttpPost]
        public IActionResult Inventory(IFormFile uploadedFile)
        {
            //получение списка объектов недвижимости
            List<Estate> estates = Parse(uploadedFile);
            if (estates == null) return View("Error");

            // вывод данных
            if (estates.Contains(Estate.ErrorEstate)) ViewBag.Message = "В ходе парсинга возникли ошибки! Полученная таблица содержит некорректные значения!";
            else ViewBag.Message = "Парсинг завершился успешно";

            ViewBag.Estates = estates;
            return View();
        }

        [NonAction]
        public List<Estate> Parse(IFormFile uploadedFile)
        {
            try
            {
                //получение файла
                string html = ReadHtmlString(uploadedFile);
                if (html == null)
                {
                    ViewBag.ErrMessage = "Ошибка загрузки файла";
                    return null;
                }

                // считывание строк
                var rows = EGRPParser.GetElements(html, "table.t tbody tr");
                if (rows == null)
                {
                    ViewBag.ErrMessage = "Ошибка чтения файла";
                    return null;
                }

                // парсинг списка имушества
                List<Estate> estates = EGRPParser.ParseAllEstates(rows);
                if (estates == null)
                {
                    ViewBag.ErrMessage = "Ошибка парсинга файла";
                    return null;
                }

                return estates;
            }
            catch (Exception)
            {
                ViewBag.ErrMessage = "Непридвиденная ошибка";
                return null;
            }
        }

        [NonAction]
        string ReadHtmlString(IFormFile uploadedFile)
        {
            try
            {
                if (uploadedFile == null || !uploadedFile.FileName.Contains(".html"))
                {
                    return null;
                }

                var stream = uploadedFile.OpenReadStream();
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                return null;
            }
            
        }
    }
}
