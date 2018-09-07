using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Baicao.Models;
using Baicao.Models.dto;

namespace Baicao.Controllers.api
{
    public class UtilController : ApiController
    {
        private ApplicationDbContext _context = null;

        public UtilController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost, Route("api/util/sendsms")]
        public IHttpActionResult sendSmsCode(SmsCodeDto dto)
        {
            Random rnd =new Random();
            SmsCode sms = new SmsCode();
            sms.PlainCode = rnd.Next(1001, 9999).ToString();
            sms.Mobiphone = dto.Mobiphone;
            sms.UserIp = HttpContext.Current.Request.UserHostAddress;
            _context.SmsCodes.Add(sms);
            _context.SaveChanges();

            //TODO: 阿里云发送SMS的code
            sms.State = 2;
            _context.SaveChanges();

            ApiResult rlt = new ApiResult()
            {
                Code = 200,
                Msg = "验证码发送成功"
            };
            return Ok(rlt);
        }
    }
}
