using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Baicao.Models;

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
                list = _context.Consumers.Where(c=>!string.IsNullOrEmpty(c.Mobilephone)).ToList();
            }
            
            var fileName = "Consumer" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
            var strRows = new StringBuilder();
            strRows.AppendLine("consumeropenid,mobiphone,regdate,couponcode,updatetime");
            string rowFormat = "{0},{1},{2},{3},{4}";
            foreach (var item in list)
            {
                strRows.AppendLine(string.Format(rowFormat,
                    item.Openid, item.Mobilephone, item.Updatetime.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.Couponcode, item.Updatetime.ToString("yyyy-MM-dd HH:mm:ss")));
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
    }
}