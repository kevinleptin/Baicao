using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Baicao.Models.dto
{
    public class VerifyPhoneResult : ApiResult
    {
        public bool Passed { get; set; }

        public string Couponcode
        {
            get;
            set;
        }

        public string Dadacode
        {
            get;
            set;
        }
    }
}