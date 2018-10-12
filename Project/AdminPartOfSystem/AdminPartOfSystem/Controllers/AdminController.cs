using AdminPartOfSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdminPartOfSystem.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        BotsAndProdsEntities1 context = new BotsAndProdsEntities1();
        public ActionResult Index()
        {
           
            return View();
        }
        public ActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddProduct(Products prod)
        {
            try
            {
                context.Insert_Product(prod.Name, prod.Price);
                return RedirectToAction("Products");
            }
            catch(Exception ex)
            {
                ViewBag.Message = @"Товар не добавлен т.к. 
он уже содержится в БД";
            }
            return View();   
        }

        public ActionResult Products()
        {

            return View(context.Products.ToList());
        }
    }
}