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
    public class DealerController : ApiController
    {
        private ApplicationDbContext _context = null;

        public DealerController()
        {
                _context = new ApplicationDbContext();
        }

        [HttpPost, Route("api/dealer/redemInfo")]
        public IHttpActionResult RedemInfo(RedemDto dto)
        {
            if (string.IsNullOrEmpty(dto.Couponcode))
            {
                return BadRequest();
            }

            string couponCode = dto.Couponcode;

            var rlt = new RedemResult();
            rlt.CouponCode = dto.Couponcode;
            var redem = _context.Redems.FirstOrDefault(c => c.CouponCode == couponCode);
            if (redem != null)
            {
                rlt.Code = 401;
                rlt.RedemDate = redem.RedemDate.ToString("yyyy-MM-dd HH:mm:ss");
                rlt.RedemPerson = redem.RedemPerson;
                rlt.RedemProduct = redem.RedemProduct;
                rlt.RedemSource = redem.RedemSource;
                rlt.Msg = "错误 - 此码已核销";
                return Ok(rlt);
            }
            else
            {
                rlt.Code = 200;
                rlt.Msg = "正常";
            }

            return Ok(rlt);
        }


        [HttpPost, Route("api/dealer/redem")]
        public IHttpActionResult Redem(RedemDto dto)
        {
            //TODO dealer通过指定页面登陆，然后在cookie中存储token或openid

            if (string.IsNullOrEmpty(dto.Couponcode))
            {
                return BadRequest();
            }
            
            string couponCode = dto.Couponcode;

            var rlt = new RedemResult();
            rlt.CouponCode = dto.Couponcode;
            var couponCodeEntity = _context.CouponCodes.FirstOrDefault(c => c.Code == couponCode);
            if (couponCodeEntity == null)
            {
                rlt.Code = 406;
                rlt.Msg = "错误 - 兑换码错误";
                return Ok(rlt);
            }

            var redem = _context.Redems.FirstOrDefault(c => c.CouponCode == couponCode);
            if (redem != null)
            {
                rlt.Code = 401;
                rlt.RedemDate = redem.RedemDate.ToString("yyyy-MM-dd HH:mm:ss");
                rlt.RedemPerson = redem.RedemPerson;
                rlt.RedemProduct = redem.RedemProduct;
                rlt.RedemSource = redem.RedemSource;
                rlt.Msg = "错误 - 此码已核销";
                return Ok(rlt);
            }

            var dealer = _context.Dealers.FirstOrDefault(c => c.RedemCode == dto.RedemCode);
            if (dealer == null)
            {
                rlt.Code = 405;
                rlt.Msg = "错误 - 核销码输入错误";
                return Ok(rlt);
            }

            redem = new Redem()
            {
                CouponCode = couponCode,
                RedemPerson = dealer.Name,
                RedemCode = dto.RedemCode,
                RedemSource = dealer.Source,
                RedemDate = DateTime.Now,
                RedemProduct = dto.Product
            };
            _context.Redems.Add(redem);
            _context.SaveChanges();

            rlt.Code = 200;
            rlt.RedemDate = redem.RedemDate.ToString("yyyy-MM-dd HH:mm:ss");
            rlt.RedemPerson = redem.RedemPerson;
            rlt.RedemProduct = redem.RedemProduct;
            rlt.RedemSource = redem.RedemSource;
            rlt.Msg = "核销成功";
            return Ok(rlt);
        }
    }
}
