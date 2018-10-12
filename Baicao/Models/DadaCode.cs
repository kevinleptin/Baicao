using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Baicao.Models
{
    public class DadaCode
    {
        public DadaCode()
        {
            CreateOn = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(10)]
        [Index(IsUnique = true)]
        public string Code { get; set; }

        public DateTime CreateOn { get; set; }

        public int BakId { get; set; }
    }
}