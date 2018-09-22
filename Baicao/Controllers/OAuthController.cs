using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Baicao.Models;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;

namespace Baicao.Controllers
{
    public class OAuthController : Controller
    {
        private string _appId = ConfigurationManager.AppSettings["wxAppId"];
        private string _appSecret = ConfigurationManager.AppSettings["wxAppSecret"];
        private ApplicationDbContext _context = null;

        public OAuthController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Login(string returnUrl)
        {
            var urlData = System.Web.HttpContext.Current.Request.Url;
            returnUrl = System.Web.HttpUtility.UrlDecode(returnUrl);
            var _oauthCallbackUrl = "/OAuth/OAuthUserInfoCallBack";
            //授权回调字符串
            var callbackUrl = string.Format("{0}://{1}{2}{3}returnUrl={4}",
                urlData.Scheme,
                urlData.Host, 
                //urlData.Port != 80 ? (":" + urlData.Port) : "",
                _oauthCallbackUrl,
                _oauthCallbackUrl.Contains("?") ? "&" : "?",
                System.Web.HttpUtility.UrlEncode(returnUrl)
            );

            var state = string.Format("{0}|{1}", "FromMeo", DateTime.Now.Ticks);
            var url = OAuthApi.GetAuthorizeUrl(_appId, callbackUrl, state, OAuthScope.snsapi_userinfo);
            return new RedirectResult(url);
        }
        
        public ActionResult OAuthUserInfoCallBack(string code, string state, string returnUrl)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Content("您拒绝了授权！");
            }

            //通过，用code换取access_token
            var userInfoAccessToken = OAuthApi.GetAccessToken(_appId, _appSecret, code);
            if (userInfoAccessToken.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + userInfoAccessToken.errmsg);
            }

            WxUserInfo wxUser = _context.WxUserInfos.FirstOrDefault(c => c.Openid == userInfoAccessToken.openid);
            //return URL应该追加上 openid，这样前端直接使用
            string openId = string.Empty;
            if (wxUser != null)
            {
                openId = System.Web.HttpUtility.UrlEncode(wxUser.Openid);
                returnUrl += (returnUrl.Contains("?") ? "&openid=" + openId : "?openid=" + openId);
                return Redirect(returnUrl);
            }

            // var wxUserInfo = CommonApi.GetUserInfo(_appId, userInfoAccessToken.openid);
            var userInfo = OAuthApi.GetUserInfo(userInfoAccessToken.access_token, userInfoAccessToken.openid);

            //把获取到的userInfo存储到数据库中
            WxUserInfo uInfo = new WxUserInfo()
            {
                //City = userInfo.city,
                //Country = userInfo.country,
                HeadImgUrl = userInfo.headimgurl,
                NickName = userInfo.nickname,
                //Sex = userInfo.sex,
                Openid = userInfo.openid,
                CreateOn = DateTime.Now
                //Privilege = string.Join(",", userInfo.privilege),
                //Province = userInfo.province,
                //UnionId = userInfo.unionid,
                //Subscribe = wxUserInfo.subscribe
            };
            var consumer = new Consumer();
            consumer.Openid = uInfo.Openid;

            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.WxUserInfos.Add(uInfo);
                    _context.Consumers.Add(consumer);
                    _context.SaveChanges();
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }

            openId = System.Web.HttpUtility.UrlEncode(uInfo.Openid);
            returnUrl += (returnUrl.Contains("?") ? "&openid=" + openId : "?openid=" + openId);
            return Redirect(returnUrl);
        }
    }
}