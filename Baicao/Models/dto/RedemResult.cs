using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Baicao.Models.dto
{
    public class RedemResult : ApiResult
    {
        public string CouponCode { get; set; }

        public string RedemSource { get; set; }
        public DateTime RedemDate { get; set; }
        public string RedemPerson { get; set; }
        public string RedemProduct { get; set; }
    }
}