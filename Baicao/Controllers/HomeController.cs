using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Baicao.Models;
using System.Data.Entity;

namespace Baicao.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //TODO: same as invitation table and redem table
        public FileResult ExpConsumer(string token)
        {
            if (string.IsNullOrEmpty(token) || token != "meoexport")
            {
                return File(Encoding.UTF8.GetBytes("未授权"), "plain/text");
            }
            // https://blog.csdn.net/sxf359/article/details/72729870 
            var list = new List<Consumer>();
            using (ApplicationDbContext _context = new ApplicationDbContext())
            {
                list = _context.Consumers.Where(c=>!string.IsNullOrEmpty(c.Mobilephone)).OrderBy(c=>c.Regdate).ToList();
            }
            
            var fileName = "Consumer" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            var strRows = new StringBuilder();
            strRows.AppendLine("consumeropenid,mobiphone,regdate,couponcode,updatetime");
            string rowFormat = "{0},{1},{2},{3},{4}";
            foreach (var item in list)
            {
                strRows.AppendLine(string.Format(rowFormat,
                    item.Openid, item.Mobilephone, item.Updatetime.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.Couponcode, item.Regdate.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            var result = strRows.ToString();

            //生成字节数组
            var fileContents = Encoding.UTF8.GetBytes(result);
            //设置excel保存到服务器的路径 
            var filePath = Server.MapPath("~/excel/" + fileName + ".csv");
            //保存excel到指定路径
            System.IO.File.WriteAllBytes(filePath, fileContents);
            // FileManager.WriteBuffToFile(fileContents, filePath);
            //读取已有的excel文件输出到客户端供客户下载该excel文件
            return File(filePath, "text/csv", fileName + ".csv");
        }

        public ActionResult Fix()
        {
            try
            {
                ApplicationDbContext _context = new ApplicationDbContext();
                int cntMax = 10;
                int cntCurrent = 0;
                while (cntCurrent < cntMax)
                {
                    var item = _context.Consumers.FirstOrDefault(c =>
                        string.IsNullOrEmpty(c.Couponcode) && !string.IsNullOrEmpty(c.Mobilephone));
                    if (item == null)
                    {
                        break;
                    }
                    var Id = item.CodeId;
                    string couponCode = _context.CouponCodes.Find(Id).Code;
                    string dadaCode = _context.DadaCodes.Find(Id).Code;
                    item.Couponcode = couponCode;
                    item.Dadacode = dadaCode;
                    _context.SaveChanges();
                    cntCurrent++;
                }
                return Content("Fix了" + cntCurrent + "条数据");
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }

        }

        public FileResult ExpCode(string token)
        {
            if (string.IsNullOrEmpty(token) || token != "meoexport")
            {
                return File(Encoding.UTF8.GetBytes("未授权"), "plain/text");
            }
            // https://blog.csdn.net/sxf359/article/details/72729870 
            var list = new List<Consumer>();
            using (ApplicationDbContext _context = new ApplicationDbContext())
            {
                list = _context.Consumers.Where(c => !string.IsNullOrEmpty(c.Mobilephone)).OrderBy(c => c.Regdate).ToList();
            }

            var fileName = "Consumer" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            var strRows = new StringBuilder();
            strRows.AppendLine("Coupon code,Dada code,mobiphone,updatetime");
            string rowFormat = "{0},{1},{2},{3}";
            foreach (var item in list)
            {
                strRows.AppendLine(string.Format(rowFormat,
                    item.Couponcode, item.Dadacode, item.Mobilephone,
                    item.Regdate.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            var result = strRows.ToString();

            //生成字节数组
            var fileContents = Encoding.UTF8.GetBytes(result);
            //设置excel保存到服务器的路径 
            var filePath = Server.MapPath("~/excel/" + fileName + ".csv");
            //保存excel到指定路径
            System.IO.File.WriteAllBytes(filePath, fileContents);
            // FileManager.WriteBuffToFile(fileContents, filePath);
            //读取已有的excel文件输出到客户端供客户下载该excel文件
            return File(filePath, "text/csv", fileName + ".csv");
        }

        public FileResult ExpInvitation(string token)
        {
            if (string.IsNullOrEmpty(token) || token != "meoexport")
            {
                return File(Encoding.UTF8.GetBytes("未授权"), "plain/text");
            }
            // https://blog.csdn.net/sxf359/article/details/72729870 
            var list = new List<Invition>();
            using (ApplicationDbContext _context = new ApplicationDbContext())
            {
                list = _context.Invitions.OrderBy(c=>c.Invdate).ToList();
            }

            var fileName = "Invitation" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            var strRows = new StringBuilder();
            strRows.AppendLine("consumeropenid,invopenid,invdate,matchtype,iftmall,tmalldate,updatetime");
            string rowFormat = "{0},{1},{2},{3},{4},{5},{6}";
            foreach (var item in list)
            {
                strRows.AppendLine(string.Format(rowFormat,
                    item.ConsumerOpenid, item.InvOpenid, item.Updatetime.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.MatchType, true, item.Updatetime.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.Updatetime.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            var result = strRows.ToString();

            //生成字节数组
            var fileContents = Encoding.UTF8.GetBytes(result);
            //设置excel保存到服务器的路径 
            var filePath = Server.MapPath("~/excel/" + fileName + ".csv");
            //保存excel到指定路径
            System.IO.File.WriteAllBytes(filePath, fileContents);
            // FileManager.WriteBuffToFile(fileContents, filePath);
            //读取已有的excel文件输出到客户端供客户下载该excel文件
            return File(filePath, "text/csv", fileName + ".csv");
        }

        public FileResult ExpRedem(string token)
        {
            if (string.IsNullOrEmpty(token) || token != "meoexport")
            {
                return File(Encoding.UTF8.GetBytes("未授权"), "plain/text");
            }
            // https://blog.csdn.net/sxf359/article/details/72729870 
            var list = new List<Redem>();
            using (ApplicationDbContext _context = new ApplicationDbContext())
            {
                list = _context.Redems.OrderBy(c=>c.RedemDate).ToList();
            }

            var fileName = "Redem" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            var strRows = new StringBuilder();
            strRows.AppendLine("couponcode,redemdate,redemsource,redemperson,redemcode,redemproduct,updatetime");
            string rowFormat = "{0},{1},{2},{3},{4},{5},{6}";
            foreach (var item in list)
            {
                strRows.AppendLine(string.Format(rowFormat,
                    item.CouponCode, item.RedemDate.ToString("yyyy-MM-dd HH:mm:ss"), item.RedemSource,
                    item.RedemPerson, item.RedemCode, item.RedemProduct, item.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            var result = strRows.ToString();
            
            //生成字节数组
            var fileContents = Encoding.UTF8.GetBytes(result);
            //设置excel保存到服务器的路径 
            var filePath = Server.MapPath("~/excel/" + fileName + ".csv");
            //保存excel到指定路径
            System.IO.File.WriteAllBytes(filePath, appendBOM(fileContents));
            // FileManager.WriteBuffToFile(fileContents, filePath);
            //读取已有的excel文件输出到客户端供客户下载该excel文件
            return File(filePath, "text/csv", fileName + ".csv");
        }

        private byte[] appendBOM(byte[] bContent)
        {
            byte[] bBOM = new byte[] { 0xEF, 0xBB, 0xBF };
            byte[] bToWrite = new byte[bBOM.Length + bContent.Length];

            //combile the BOM and the content
            bBOM.CopyTo(bToWrite, 0);
            bContent.CopyTo(bToWrite, bBOM.Length);
            return bToWrite;
        }
    }
}