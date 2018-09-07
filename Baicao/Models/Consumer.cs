using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Baicao.Models
{
    public class Consumer
    {
        public Consumer()
        {
            Updatetime = DateTime.Now;
            Regdate = DateTime.Now;
            if (HttpContext.Current != null)
            {
                Userip = HttpContext.Current.Request.UserHostAddress;
            }
        }
        [Key]
        public string Openid { get; set; }
        [MaxLength(20)]
        public string Mobilephone { get; set; }
        public DateTime Regdate { get; set; }
        [MaxLength(32)]
        public string Couponcode { get; set; }
        public DateTime Updatetime { get; set; }
        [MaxLength(32)]
        public string Userip { get; set; }
        [MaxLength(10)]
        public string Dadacode
        {
            get;
            set;
        }

    }
}