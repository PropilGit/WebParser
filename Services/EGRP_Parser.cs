using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebParser.Models.EGRP;

namespace WebParser.Services
{
    public class EGRP_Parser
    {
        protected string ReadString(IFormFile uploadedFile, string contentType)
        {
            try
            {
                if (uploadedFile == null || !(uploadedFile.ContentType == contentType)) //!uploadedFile.FileName.Contains(".html")
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
