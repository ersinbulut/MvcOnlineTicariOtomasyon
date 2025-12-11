using MvcOnlineTicariOtomasyon.Models.Siniflar;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    public class CariController : Controller
    {
        Context c = new Context();
        // GET: Car
        public ActionResult Index(string search, string city, int page = 1, int pageSize = 10)
        {
            var cariler = c.Carilers.AsQueryable();

            // Arama
            if (!string.IsNullOrEmpty(search))
            {
                cariler = cariler.Where(x =>
                    x.CariAd.Contains(search) ||
                    x.CariSoyad.Contains(search) ||
                    x.CariMail.Contains(search));
            }

            // Şehir filtresi
            if (!string.IsNullOrEmpty(city))
            {
                cariler = cariler.Where(x => x.CariSehir == city);
            }

            // Şehir listesi (ViewBag'e aktar)
            ViewBag.Sehirler = c.Carilers
                                 .Select(x => x.CariSehir)
                                 .Distinct()
                                 .OrderBy(x => x)
                                 .ToList();

            ViewBag.Search = search;
            ViewBag.City = city;
            ViewBag.PageSize = pageSize;

            return View(cariler.ToList().ToPagedList(page, pageSize));
        }

        [HttpGet]
        public ActionResult YeniCari()
        {
            return View();
        }
        [HttpPost]
        public ActionResult YeniCari(Cariler p)
        {
            p.Durum = true;
            c.Carilers.Add(p);
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult CariSil(int id)
        {
            var cari = c.Carilers.Find(id);
            cari.Durum = false;
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult CariGetir(int id)
        {
            var cari = c.Carilers.Find(id);
            return View("CariGetir", cari);
        }
        public ActionResult CariGuncelle(Cariler p)
        {
            var cari = c.Carilers.Find(p.CariID);
            cari.CariAd = p.CariAd;
            cari.CariSoyad = p.CariSoyad;
            cari.CariSehir = p.CariSehir;
            cari.CariMail = p.CariMail;
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult MusteriSatis(int id)
        {
            var degerler = c.satisHarekets.Where(x => x.CariID == id).ToList();
            var cr = c.Carilers.Where(x => x.CariID == id).Select(y => y.CariAd + " " + y.CariSoyad).FirstOrDefault();
            ViewBag.cari = cr;
            return View(degerler);
        }
    }
}