using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebParser.Models.EGRP;

namespace WebParser.Services
{
    public interface IEGRP_Parser
    {
        public (List<Estate>, string) Parse(IFormFile uploadedFile);
    }
}
