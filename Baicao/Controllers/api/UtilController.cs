using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
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

            SendSms(dto.Mobiphone, sms.PlainCode);
            
            sms.State = 2;
            _context.SaveChanges();

            ApiResult rlt = new ApiResult()
            {
                Code = 200,
                Msg = "验证码发送成功"
            };
            return Ok(rlt);
        }

        private SendSmsResponse SendSms(string phoneNumber, string verifyCode)
        {
            //产品名称:云通信短信API产品,开发者无需替换
            const String product = "Dysmsapi";
            //产品域名,开发者无需替换
            const String domain = "dysmsapi.aliyuncs.com";

            string accessKeyId = ConfigurationManager.AppSettings["accessKeyId"];
            string accessKeySecret = ConfigurationManager.AppSettings["accessKeySecret"];
            string templateCode = ConfigurationManager.AppSettings["templateCode"];
            string signName = ConfigurationManager.AppSettings["signName"];

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            SendSmsResponse response = null;
            try
            {

                //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                request.PhoneNumbers = phoneNumber;
                //必填:短信签名-可在短信控制台中找到
                request.SignName = signName;
                //必填:短信模板-可在短信控制台中找到
                request.TemplateCode = templateCode;
                //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                request.TemplateParam = "{\"code\":\""+verifyCode+"\"}";
                //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                request.OutId = "yourOutId";
                //请求失败这里会抛ClientException异常
                response = acsClient.GetAcsResponse(request);

            }
            catch (ServerException e)
            {
                //TODO: Log
                Console.WriteLine(e.ErrorCode);
            }
            catch (ClientException e)
            {
                //TODO: Log
                Console.WriteLine(e.ErrorCode);
            }
            return response;
        }
    }
}
