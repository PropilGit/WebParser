﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebParser.Models.EGRP
{
    public class Estate
    {
        public int Number { get; private set; }
        public static string NumberTitle = "№ п/п";

        public string KadastrNum { get; private set; }
        public static string KadastrNumTitle = "Кадастровый (или условный) номер объекта";

        public string Name
        {
            get => name;
            private set
            {
                name = value.ToLower();
            }
        }
        private string name;
        public static string NameTitle = "наименование объекта";

        public string Purpose { 
            get => purpose; 
            private set {
                purpose = Regex.Replace(value, @"^[0-9|)|.]+", "").Trim();
            }
        }
        private string purpose;
        public static string PurposeTitle = "назначение объекта";

        public string Area { get; private set; }
        public static string AreaTitle = "площадь объекта";

        public string Address { get; private set; }
        public static string AddressTitle = "адрес (местоположение) объекта";

        public string RightType { get; private set; }
        public static string RightTypeTitle = "Вид права, доля в праве";

        public string GosRegDate { get; private set; }
        public static string GosRegDateTitle = "дата государственной регистрации:";

        public string GosRegNum { get; private set; }
        public static string GosRegNumTitle = "номер государственной регистрации:";

        public string GosRegBasis { get; private set; }
        public static string GosRegBasisTitle = "основание государственной регистрации:";

        public List<RightsRestriction> RightsRestrictions { get; private set; }
        public static string RightsRestrictionsTitle = "Ограничения права";
        public string RightsRestrictionsToString
        {
            get
            {
                string rrString = "";
                foreach (var rr in RightsRestrictions)
                {
                    rrString += rr.ToString();
                }
                return rrString;
            }
        }

        public Estate(int number, string kadastrNum, string name, string purpose, string area, string address,
            string rightType, string gosRegDate, string gosRegNum, string gosRegBasis, List<RightsRestriction> rightsRestrictions)
        {

            Number = number;
            KadastrNum = NormalizeString(kadastrNum);
            Name = NormalizeString(name);
            Purpose = NormalizeString(purpose);
            Area = NormalizeString(area);
            Address = NormalizeString(address);
            RightType = NormalizeString(rightType);
            GosRegDate = NormalizeString(gosRegDate);
            GosRegNum = NormalizeString(gosRegNum);
            GosRegBasis = NormalizeString(gosRegBasis);
            RightsRestrictions = rightsRestrictions;
        }

        public struct RightsRestriction
        {
            //public int Number { get; private set; }

            // вид:
            public string Type { get; private set; }

            // номер государственной регистрации:
            public string GosRegNum { get; private set; }

            public RightsRestriction(string type, string gosRegNum)
            {
                Type = NormalizeString(type);
                GosRegNum = NormalizeString(gosRegNum);
            }

            public string ToString()
            {
                return Type + ", " + GosRegNum + "; ";
            }
        }

        public static Estate ErrorEstate = new Estate(
            0, "---", "---", "---", "---", "---", "---", "---", "---", "---",
            new List<Estate.RightsRestriction> { new Estate.RightsRestriction("---", "---") });

        static string NormalizeString(string input)
        {
            string[] replaceables = new[] { "\n", "\t", "\r", " " };
            string rxString = string.Join("|", replaceables.Select(s => Regex.Escape(s)));

            string result = Regex.Replace(input, rxString, " ");
            result = Regex.Replace(result, @"[ ]{2,}", " ");

            if (result == " ") 
                result = "";

            return result;
        }
    }
}
