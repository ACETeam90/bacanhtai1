using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace S4T_HaTinh.Controllers
{
    public class HT_QuanLyMoiTruongChinhSachController : Controller
    {
        // GET: HT_QuanLyMoiTruongChinhSach
        public ActionResult Index()
        {
            return View();
        }

        // GET: HT_QuanLyMoiTruongChinhSach/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HT_QuanLyMoiTruongChinhSach/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HT_QuanLyMoiTruongChinhSach/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: HT_QuanLyMoiTruongChinhSach/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HT_QuanLyMoiTruongChinhSach/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: HT_QuanLyMoiTruongChinhSach/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HT_QuanLyMoiTruongChinhSach/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
