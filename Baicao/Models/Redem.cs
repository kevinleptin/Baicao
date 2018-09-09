using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Baicao.Models
{
    public class Redem
    {
        public Redem()
        {
            RedemDate = DateTime.Now;
            UpdateTime = DateTime.Now;
        }
        [MaxLength(16)]
        [Key]
        public string CouponCode { get; set; }
        public DateTime RedemDate { get; set; }
        [MaxLength(32)]
        public string RedemSource { get; set; }
        [MaxLength(32)]
        public string RedemPerson { get; set; }
        [MaxLength(32)]
        public string RedemCode { get; set; }
        [MaxLength(32)]
        public string RedemProduct { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}