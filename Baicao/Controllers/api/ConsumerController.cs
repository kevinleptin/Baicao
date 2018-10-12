﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Baicao.Models;
using Baicao.Models.dto;
using System.Data.Entity;
using System.Web;

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
            if (consumer == null || string.IsNullOrEmpty(consumer.Couponcode))
            {//没有赋值CODE，不能算参与成功
                rlt.Code = 200;
                rlt.IsJoined = false;
                rlt.Msg = "用户还未验证手机号码";
            }
            else
            {
                rlt.Code = 200;
                rlt.IsJoined = true;
                rlt.Msg = "用户已验证手机号码";
                rlt.Couponcode = consumer.Couponcode;
                rlt.Dadacode = consumer.Dadacode;
            }

            return Ok(rlt);
        }

        [HttpPost, Route("api/consumer/fix")]
        public IHttpActionResult Clear()
        {
            ApiResult rlt = new ApiResult();
            

            return Ok(rlt);
        }

        [HttpPost, Route("api/consumer/userauth")]
        public IHttpActionResult UserAuth(WxUserInfoDto dto)
        {
            IsJoinedResult rlt = new IsJoinedResult();
            if (dto == null || dto.OpenId == null)
            {
                rlt.IsJoined = false;
                rlt.Code = 410;
                rlt.Msg = "参数错误";
                return Ok(rlt);
            }
            var wxUserInfo = _context.WxUserInfos.FirstOrDefault(c => c.Openid == dto.OpenId);
            if (wxUserInfo == null)
            {
                using (var trans = _context.Database.BeginTransaction())
                {
                    try
                    {
                        wxUserInfo = new WxUserInfo();
                        wxUserInfo.Openid = dto.OpenId;
                        wxUserInfo.HeadImgUrl = HttpUtility.UrlDecode(dto.HeadImgUrl);
                        wxUserInfo.NickName = HttpUtility.UrlDecode(dto.NickName);

                        var consumer = new Consumer();
                        consumer.Openid = dto.OpenId;

                        _context.Consumers.Add(consumer);
                        // var consumerInfo = _context.Consumers.FirstOrDefault(c=>c.Openid)

                        _context.WxUserInfos.Add(wxUserInfo);
                        _context.SaveChanges();

                        trans.Commit();
                    }
                    catch(Exception e)
                    {
                        trans.Rollback();
                        //TODO: 返回错误 code 500 记录日志
                    }
                }
            }
            else
            {
                var consumer = _context.Consumers.First(c => c.Openid == dto.OpenId);
                if (!string.IsNullOrEmpty(consumer.Mobilephone))
                {
                    rlt.Code = 200;
                    rlt.IsJoined = true;
                    rlt.Msg = "用户已验证手机号码";
                }
            }

            return Ok(rlt);

        }

        [HttpPost, Route("api/consumer/join")]
        public IHttpActionResult Join(VerifyPhoneDto dto)
        {
            VerifyPhoneResult rlt = new VerifyPhoneResult();
            if (string.IsNullOrEmpty(dto.Openid))
            {
                rlt.Code = 400;
                rlt.Msg = "数据格式错误";
                return Ok(rlt);
            }

            var smsCode = _context.SmsCodes.OrderByDescending(c => c.Id)
                .FirstOrDefault(c => c.Mobiphone == dto.Mobiphone && c.IsUsed == false);
            if (smsCode == null)
            {
                rlt.Code = 401;
                rlt.Msg = "错误：短信验证码未获取";
                return Ok(rlt);
            }

            var ts = DateTime.Now - smsCode.CreateOn;
            if (ts.TotalMinutes >= 5)
            {
                rlt.Code = 403;
                rlt.Msg = "验证码已超时";
                return Ok(rlt);
            }

            if (smsCode.PlainCode != dto.VerifyCode)
            {
                rlt.Code = 404;
                rlt.Msg = "短信验证码错误";
                return Ok(rlt);
            }

            var csm = _context.Consumers.FirstOrDefault(c => c.Openid == dto.Openid);
            if (!string.IsNullOrEmpty(csm.Couponcode))
            { //发现14例用户没有获取到CODE,改判断条件，让用户能重试.
                rlt.Code = 402;
                rlt.Msg = "重复参与";
                return Ok(rlt);
            }

            
            csm.Mobilephone = dto.Mobiphone;
            csm.Userip = System.Web.HttpContext.Current.Request.UserHostAddress;
            csm.Regdate = DateTime.Now;
            smsCode.IsUsed = true;
            _context.SaveChanges();

            rlt.Code = 200;
            rlt.Msg = "注册成功";
            rlt.Passed = true;
            string dadaCode = string.Empty;
            string couponCode = string.Empty;

            var couponCodeEntity = _context.CouponCodes.FirstOrDefault(c=>c.BakId == csm.CodeId);
            var dadaCodeEntity = _context.DadaCodes.First(c=>c.BakId == csm.CodeId);
            rlt.Dadacode = dadaCodeEntity.Code;
            rlt.Couponcode = couponCodeEntity.Code;

            csm.Dadacode = rlt.Dadacode;
            csm.Couponcode = rlt.Couponcode;
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
            MatchResult rlt = new MatchResult();
            var csm = _context.Consumers.Single(c => c.Openid == dto.openid);
            var otherCsm = _context.Consumers.SingleOrDefault(c => c.Dadacode == dto.dadacode);
            if(otherCsm == null) {
                rlt.Code = 400;
                rlt.Msg = "拼搭失败 - 拼搭码未找到";
                return Ok(rlt);
            }

            if (otherCsm.Openid.Equals(dto.openid, StringComparison.OrdinalIgnoreCase))
            {
                rlt.Code = 402;
                rlt.Msg = "拼搭失败 - 不能拼搭自己";
                return Ok(rlt);
            }

            //fix: 两人不能交叉拼搭
            var invition = _context.Invitions
                .FirstOrDefault(c => 
                    (c.ConsumerOpenid == dto.openid && c.InvOpenid == otherCsm.Openid) ||
                    (c.ConsumerOpenid == otherCsm.Openid && c.InvOpenid == dto.openid));
            if(invition != null) {
                rlt.Code = 401;
                rlt.Msg = "重复拼搭";
                return Ok(rlt);
            }
            Random rnd = new Random();
            invition = new Invition()
            {
                ConsumerOpenid = dto.openid,
                InvOpenid = otherCsm.Openid,
                Iftmall = true,
                MatchType = rnd.Next(0, 6)
            };
            _context.Invitions.Add(invition);
            _context.SaveChanges();
            rlt.Code = 200;
            rlt.Msg = "拼搭成功";
            rlt.MatchType = invition.MatchType;
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
                                      ).Take(50).ToList();

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
