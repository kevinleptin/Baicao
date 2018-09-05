using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Baicao.Models;
using Baicao.Models.dto;

namespace Baicao.Controllers.api
{
    public class ConsumerController : ApiController
    {
        private ApplicationDbContext _context = null;

        public ConsumerController()
        {
            _context = new ApplicationDbContext();
        }
        [HttpPost, Route("api/consumer/isjoined")]
        public IHttpActionResult IsJoined(WxOpenIdObj dto)
        {
            IsJoinedResult rlt = new IsJoinedResult();
            if (dto == null || string.IsNullOrEmpty(dto.Openid))
            {
                rlt.Code = 400;
                rlt.IsJoined = false;
                rlt.Msg = "无效的openid";
                return Ok(rlt);
            }

            var consumer = _context.Consumers.FirstOrDefault(c => c.Openid == dto.Openid);
            if (consumer == null || string.IsNullOrEmpty(consumer.Mobilephone))
            {
                rlt.Code = 200;
                rlt.IsJoined = false;
                rlt.Msg = "用户还未验证手机号码";

                if (consumer == null)
                {
                    consumer = new Consumer();
                    consumer.Openid = dto.Openid;
                    consumer.Userip = System.Web.HttpContext.Current.Request.UserHostAddress;
                    _context.Consumers.Add(consumer);
                    _context.SaveChanges();
                }
            }
            else
            {
                rlt.Code = 200;
                rlt.IsJoined = true;
                rlt.Msg = "用户已验证手机号码";
            }

            return Ok(rlt);
        }

        public IHttpActionResult Join(VerifyPhoneDto dto)
        {
            VerifyPhoneResult rlt = new VerifyPhoneResult();
            //TODO: 验证dto

            return Ok(rlt);
        }
    }
}
