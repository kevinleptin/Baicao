using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Baicao.Models
{
    public class Invition
    {
        public Invition()
        {
            Updatetime = DateTime.Now;
            Invdate = DateTime.Now;
            Tmalldate = DateTime.Now;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id
        {
            get;
            set;
        }


        public WxUserInfo Consumer
        {
            get;
            set;
        }

        public string ConsumerOpenid
        {
            get;
            set;
        }

        public WxUserInfo Inv
        {
            get;
            set;
        }

        public string InvOpenid
        {
            get;
            set;
        }

        public DateTime Invdate
        {
            get;
            set;
        }

        public int MatchType
        {
            get;
            set;
        }

        public bool Iftmall
        {
            get;
            set;
        }

        public DateTime Tmalldate
        {
            get;
            set;
        }

        public DateTime Updatetime
        {
            get;
            set;
        }
    }
}
