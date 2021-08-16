using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using WebParser.Models.EGRP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebParser.Controllers
{
    public class EGRPController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Parse(IFormFile uploadedFile)
        {
            //получение файла
            string html = ReadHtmlString(uploadedFile);
            if (html == null)
            {
                ViewBag.Message = "Ошибка загрузки файла";
                return View("Error");
            }

            // считывание строк
            var rows = GetElements(html, "table.t tbody tr");
            if (rows == null)
            {
                ViewBag.Message = "Ошибка чтения файла";
                return View("Error");
            }

            // парсинг списка имушества
            List<Estate> estates = ParseAllEstates(rows);
            if (estates == null)
            {
                ViewBag.Message = "Ошибка парсинга файла";
                return View("Error");
            }

            // вывод данных
            if (estates.Contains(Estate.ErrorEstate)) ViewBag.Message = "В ходе парсинга возникли ошибки! Полученная таблица содержит некорректные значения!";
            else ViewBag.Message = "Парсинг завершился успешно";
            ViewBag.Estates = estates;
            return View("Result");
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

        [NonAction]
        IHtmlCollection<IElement> GetElements(string html, string selector)
        {
            try
            {
                var parser = new HtmlParser();
                var document = parser.ParseDocument(html);

                return document.QuerySelectorAll(selector);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region Parse

        [NonAction]
        List<Estate> ParseAllEstates(IHtmlCollection<IElement> rows)
        {
            try
            {
                int estateCounter = 1;
                List<Estate> estates = new List<Estate>();
                for (int r = 0; r < rows.Length;)
                {
                    int rowspan = Int32.Parse(rows[r].QuerySelector("td:nth-child(1)").GetAttribute("rowspan"));

                    Estate estate = ParseSingleEstate(
                        estateCounter,
                        rows.Skip(r).Take(rowspan));

                    if (estate == null)
                    {
                        estates.Add(Estate.ErrorEstate);
                    }
                    else
                    {
                        estates.Add(estate);
                    }
                    estateCounter++;

                    r += rowspan;
                }

                return estates;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [NonAction]
        Estate ParseSingleEstate(int estateIndex, IEnumerable<IElement> rows)
        {
            try
            {
                // элементы ДО ограничения права

                string kadastrNum = rows.ElementAt(0).QuerySelector("td:nth-child(4)").Text();
                string name = rows.ElementAt(1).QuerySelector("td:nth-child(2)").Text();
                string purpose = rows.ElementAt(2).QuerySelector("td:nth-child(2)").Text();
                string area = rows.ElementAt(3).QuerySelector("td:nth-child(2)").Text();
                string address = rows.ElementAt(4).QuerySelector("td:nth-child(2)").Text();
                string rightType = rows.ElementAt(5).QuerySelector("td:nth-child(3)").Text();
                string gosRegDate = rows.ElementAt(6).QuerySelector("td:nth-child(2)").Text();
                string gosRegNum = rows.ElementAt(7).QuerySelector("td:nth-child(2)").Text();
                string gosRegBasis = rows.ElementAt(8).QuerySelector("td:nth-child(2)").Text();

                //ограничение права
                List<Estate.RightsRestriction> rR = new List<Estate.RightsRestriction>();
                for (int e = 10; e < rows.Count(); e += 2)
                {
                    string rRType = rows.ElementAt(e).QuerySelector("td:nth-child(3)").Text();
                    string rRGosRegNum = rows.ElementAt(e + 1).QuerySelector("td:nth-child(2)").Text();

                    rR.Add(new Estate.RightsRestriction(rRType, rRGosRegNum));
                }

                return new Estate(estateIndex, kadastrNum, name, purpose, area, address, rightType, gosRegDate, gosRegNum, gosRegBasis, rR);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
