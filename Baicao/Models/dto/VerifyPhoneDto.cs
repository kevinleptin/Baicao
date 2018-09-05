using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Baicao.Models.dto
{
    public class VerifyPhoneDto
    {
        public string Mobiphone { get; set; }
        public string Openid { get; set; }
        public string VerifyCode { get; set; }
    }
}