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

        public ActionResult Login()
        {
            var urlData = System.Web.HttpContext.Current.Request.Url;
            var returnUrl = urlData.ToString(); //todo: 这里是外面传来的URL值
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

            //if (!state.Contains("|"))
            //{
            //    //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下
            //    //实际上可以存任何想传递的数据，比如用户ID
            //    return Content("验证失败！请从正规途径进入！1001");
            //}

            //通过，用code换取access_token
            var userInfoAccessToken = OAuthApi.GetAccessToken(_appId, _appSecret, code);
            if (userInfoAccessToken.errcode != ReturnCode.请求成功)
            {
                return Content("错误：" + userInfoAccessToken.errmsg);
            }

            WxUserInfo wxUser = _context.WxUserInfos.FirstOrDefault(c => c.Openid == userInfoAccessToken.openid);
            //TODO: return URL应该追加上 openid，这样前端直接使用

            if (wxUser != null)
            {
                return Redirect(returnUrl);
            }

            var wxUserInfo = CommonApi.GetUserInfo(_appId, userInfoAccessToken.openid);
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
            
            _context.WxUserInfos.Add(uInfo);
            _context.SaveChanges();

            return Redirect(returnUrl);
        }
    }
}