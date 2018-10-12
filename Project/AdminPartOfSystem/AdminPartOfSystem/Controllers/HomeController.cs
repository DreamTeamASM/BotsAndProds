using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdminPartOfSystem.Models;
namespace AdminPartOfSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string login,string pass)
        {
            BotsAndProdsEntities1 context = new BotsAndProdsEntities1();
          
            List<Authorize_Result> per= context.Authorize(login, pass).ToList();
            if (per.Count>0)
            {
                return RedirectToAction("Index", "Admin");
            }
            
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
    }
}