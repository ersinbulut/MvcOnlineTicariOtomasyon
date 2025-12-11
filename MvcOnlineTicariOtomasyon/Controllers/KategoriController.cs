using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcOnlineTicariOtomasyon.Models.Siniflar;
using PagedList;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    public class KategoriController : Controller
    {
        // GET: Kategori
        Context c = new Context();
        public ActionResult Index(string search, int? kategoriId, int page = 1, int pageSize = 10)
        {
            var liste = c.Kategoris.AsQueryable();

            // Arama filtresi
            if (!string.IsNullOrEmpty(search))
            {
                liste = liste.Where(x => x.KategoriAd.Contains(search));
            }

            // Kategoriye göre filtre (Kategori tablosu varsa kullanılır)
            if (kategoriId != null)
            {
                liste = liste.Where(x => x.KategoriID == kategoriId);
            }

            liste = liste.OrderBy(x => x.KategoriID);

            var paged = liste.ToPagedList(page, pageSize);
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentKategori = kategoriId;
            ViewBag.PageSize = pageSize;
            ViewBag.Kategoriler = c.Kategoris.ToList();


            return View(paged);
        }





        [HttpGet]
        public ActionResult KategoriEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult KategoriEkle(Kategori k)
        {
            c.Kategoris.Add(k);
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult KategoriSil(int id)
        {
            var ktg = c.Kategoris.Find(id);
            c.Kategoris.Remove(ktg);
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult KategoriGetir(int id)
        {
            var kategori = c.Kategoris.Find(id);
            return View("KategoriGetir", kategori);
        }
        public ActionResult KategoriGuncelle(Kategori k)
        {
            var ktgr = c.Kategoris.Find(k.KategoriID);
            ktgr.KategoriAd = k.KategoriAd;
            c.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}