using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Baicao.Models;
using Baicao.Models.dto;
using System.Data.Entity;

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
            //todo Couponcode和Dadacode需要保持数据库唯一
            rlt.Dadacode = GenerateRndCode(6);
            rlt.Couponcode = GenerateRndCode(7);

            csm.Couponcode = rlt.Couponcode;
            csm.Dadacode = rlt.Dadacode;
            csm.Mobilephone = dto.Mobiphone;
            csm.Userip = System.Web.HttpContext.Current.Request.UserHostAddress;
            csm.Regdate = DateTime.Now;
            _context.SaveChanges();

            return Ok(rlt);
        }

        [HttpPost, Route("api/consumer/info")]
        public IHttpActionResult Info(KeyDto dto) {
            ConsumerInfoResult rlt = new ConsumerInfoResult();
            var csm = _context.Consumers.FirstOrDefault(c => c.Openid == dto.Openid);
            if(csm == null) {
                rlt.Code = 404;
                rlt.Msg = "没有数据";
                return Ok(rlt);
            }

            rlt.Code = 200;
            rlt.Couponcode = csm.Couponcode;
            rlt.Dadacode = csm.Dadacode;
            return Ok(rlt);
        }

        [HttpPost, Route("api/consumer/match")]
        public IHttpActionResult Match(matchdto dto) {
            ApiResult rlt = new ApiResult();
            var csm = _context.Consumers.Single(c => c.Openid == dto.openid);
            var otherCsm = _context.Consumers.SingleOrDefault(c => c.Dadacode == dto.dadacode);
            if(otherCsm == null) {
                rlt.Code = 400;
                rlt.Msg = "拼搭失败 - 拼搭码未找到";
                return Ok(rlt);
            }

            var invition = _context.Invitions.FirstOrDefault(c => c.Invopenid == dto.openid && c.Invopenid == otherCsm.Openid);
            if(invition != null) {
                rlt.Code = 401;
                rlt.Msg = "重复拼搭";
                return Ok(rlt);
            }
            Random rnd = new Random();
            invition = new Invition()
            {
                Consumeropenid = dto.openid,
                Invopenid = otherCsm.Openid,
                Iftmall = true,
                MatchType = rnd.Next(0, 6)
            };
            _context.Invitions.Add(invition);
            _context.SaveChanges();
            rlt.Code = 200;
            rlt.Msg = "拼搭成功";
            return Ok(rlt);
        }

        [HttpPost, Route("api/consumer/matchlist")]
        public IHttpActionResult MatchList(matchdto dto)
        {
            var list = _context.Invitions.Include(c => c.Consumer).Include(c => c.Inv)
                     .Where(c => c.ConsumerOpenid == dto.openid || c.InvOpenid == dto.openid).OrderByDescending(c => c.Id)
                               .Select(c => new UserInfoSimple
                               {
                                   Nickname = (c.ConsumerOpenid == dto.openid) ? c.Inv.NickName : c.Consumer.NickName,
                                   HeadImgUrl = (c.ConsumerOpenid == dto.openid) ? c.Inv.HeadImgUrl : c.Consumer.HeadImgUrl,
                                   InvitionType = (c.ConsumerOpenid == dto.openid) ? "host" : "client",
                                   Invdate = c.Invdate,
                                   MatchType = c.MatchType

                               }
                                      ).ToList();

            var rlt = new MatchlistResult();
            rlt.Code = 200;
            rlt.Msg = "";
            rlt.List = list;

            return Ok(rlt);
        }

        private string GenerateRndCode(int length)
        {
            string rnd = Guid.NewGuid().ToString().Replace("-","").Substring(0, length);
            return rnd;
        }
    }
}
