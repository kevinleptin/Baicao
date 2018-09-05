using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Baicao.Models
{
    public class SmsCode
    {
        public SmsCode()
        {
            CreateOn = DateTime.Now;
            State = -1;
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public DateTime CreateOn { get; set; }

        [MaxLength(32)]
        public string UserIp { get; set; }

        [MaxLength(20)]
        public string Mobiphone { get; set; }

        /// <summary>
        /// 高级安全等级，数据库不存储明文短信验证码。
        /// </summary>
        [MaxLength(32)]
        public string CodeHash { get; set; }

        [MaxLength(32)]
        public string Salt { get; set; }

        /// <summary>
        /// 默认安全等级使用明文码，降低服务器压力
        /// </summary>
        [MaxLength(16)]
        public string PlainCode { get; set; }

        /// <summary>
        /// 发送状态，用于存储短信服务器返回的值
        /// </summary>
        public int State { get; set; }
    }
}