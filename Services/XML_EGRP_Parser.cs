using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using WebParser.Models.EGRP;

namespace WebParser.Services
{
    public class XML_EGRP_Parser: EGRP_Parser, IEGRP_Parser
    {
        public (List<Estate>, string) Parse(IFormFile uploadedFile)
        {
            try
            {
                //string xmlString = ReadString(uploadedFile, "text/xml");
                //if (xmlString == null) return (null, "Ошибка загрузки файла");

                XPathDocument xpathDoc = new XPathDocument(uploadedFile.OpenReadStream());
                XPathNavigator navigator = xpathDoc.CreateNavigator();

                //XmlDocument xml = new XmlDocument();
                //xml.LoadXml(xmlString);
                //var baseNode = xml.SelectSingleNode(./)

                List<Estate> estates = new List<Estate>();
                //land_record
                XPathNodeIterator land_records = navigator.Select("/extract_rights_individ_available_real_estate_objects/base_data/land_records");
                foreach (var item in land_records)
                {
                    Console.WriteLine(item);
                }
                Console.ReadLine();
                return (null, "debug");
                
                //foreach (var node in xml.DocumentElement.ChildNodes)
                //{
                //    Sele
                //}
                /*
                // считывание строк
                var rows = GetElements(html, "table.t tbody tr");
                if (rows == null) return (null, "Ошибка чтения файла");

                // парсинг списка имушества
                List<Estate> estates = ParseAllEstates(rows);
                if (estates == null) return (null, "Ошибка парсинга файла");

                return (estates, "Парсинг завершился успешно");
                */
            }
            catch (Exception)
            {
                return (null, "Непридвиденная ошибка");
            }
        }
    }
}
