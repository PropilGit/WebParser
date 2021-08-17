using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebParser.Models.EGRP;

namespace WebParser.Services
{
    public class EGRPParser
    {

        public static IHtmlCollection<IElement> GetElements(string html, string selector)
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

        public static List<Estate> ParseAllEstates(IHtmlCollection<IElement> rows)
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

        public static Estate ParseSingleEstate(int estateIndex, IEnumerable<IElement> rows)
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

    }
}
