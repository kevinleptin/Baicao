using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Baicao.Models.dto
{
    public class ApiResult
    {
        public int Code { get; set; }
        public string Msg { get; set; }

        public ApiResult()
        {
            Code = 200;
            Msg = "";
        }
    }
}