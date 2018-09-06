using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Baicao.Models
{
    public class WxUserInfo
    {
        public WxUserInfo()
        {
        }

        [Key]
        public string Openid
        {
            get;
            set;
        }

        public string NickName
        {
            get;
            set;
        }

        public string HeadImgUrl
        {
            get;
            set;
        }

        public DateTime CreateOn
        {
            get;
            set;
        }

        //todo 存储更多信息
    }
}
