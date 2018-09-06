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

        [HttpPost, Route("api/consumer/join")]
        public IHttpActionResult Join(VerifyPhoneDto dto)
        {
            VerifyPhoneResult rlt = new VerifyPhoneResult();
            if(string.IsNullOrEmpty(dto.Openid)) {
                rlt.Code = 400;
                rlt.Msg = "数据格式错误";
                return Ok(rlt);
            }

            var smsCode = _context.SmsCodes.LastOrDefault(c => c.Mobiphone == dto.Mobiphone && c.IsUsed == false);
            if(smsCode == null) {
                rlt.Code = 401;
                rlt.Msg = "错误：短信验证码未获取";
                return Ok(rlt);
            }

            if(smsCode.PlainCode != dto.VerifyCode) {
                rlt.Code = 400;
                rlt.Msg = "短信验证码错误";
                return Ok(rlt);
            }

            var csm = _context.Consumers.FirstOrDefault(c => c.Openid == dto.Openid);
            if(!string.IsNullOrEmpty(csm.Mobilephone)) {
                rlt.Code = 402;
                rlt.Msg = "重复参与";
                return Ok(rlt);
            }

            rlt.Code = 200;
            rlt.Msg = "注册成功";
            rlt.Passed = true;
            rlt.Dadacode = GenerateRndCode(6);
            rlt.Dadacode = GenerateRndCode(7);

            //todo 存储到数据库

            return Ok(rlt);
        }

        private string GenerateRndCode(int length)
        {
            string rnd = Guid.NewGuid().ToString().Replace("-","").Substring(0, length);
            return rnd;
        }
    }
}
