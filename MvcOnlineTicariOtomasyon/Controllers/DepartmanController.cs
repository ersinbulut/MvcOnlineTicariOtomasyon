using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcOnlineTicariOtomasyon.Models.Siniflar;
using PagedList;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    public class DepartmanController : Controller
    {
        Context c = new Context();
        // GET: Departman
        public ActionResult Index(string search, int page = 1, int pageSize = 10)
        {
            var list = c.Departmans.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                list = list.Where(x => x.DepartmanAd.Contains(search));

            list = list.OrderBy(x => x.DepartmanID);

            ViewBag.CurrentSearch = search;
            ViewBag.PageSize = pageSize;

            return View(list.ToPagedList(page, pageSize));
        }

        [HttpGet]
        public ActionResult DepartmanEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DepartmanEkle(Departman d)
        {
            c.Departmans.Add(d);
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult DepartmanSil(int id)
        {
            var dep = c.Departmans.Find(id);
            dep.Durum = false;
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult DepartmanGetir(int id)
        {
            var dpt = c.Departmans.Find(id);
            return View("DepartmanGetir",dpt);
        }
        public ActionResult DepartmanGuncelle(Departman p)
        {
            var dept = c.Departmans.Find(p.DepartmanID);
            dept.DepartmanAd = p.DepartmanAd;
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult DepartmanDetay(int id)
        {
            var degerler = c.Personels.Where(x => x.DepartmanID == id).ToList();
            var dpt = c.Departmans.Where(x => x.DepartmanID == id).Select(y => y.DepartmanAd).FirstOrDefault();
            ViewBag.d = dpt;
            return View(degerler);
        }
        public ActionResult DepartmanPersonelSatis(int id, string urun, string cari, int page = 1, int pageSize = 10)
        {
            var personel = c.Personels.Where(x => x.PersonelID == id)
                .Select(x => x.PersonelAd + " " + x.PersonelSoyad)
                .FirstOrDefault();

            ViewBag.dpers = personel;

            var satislar = c.satisHarekets.Where(x => x.PersonelID == id).AsQueryable();

            // Ürün filtreleme
            if (!string.IsNullOrEmpty(urun))
            {
                satislar = satislar.Where(x => x.Urun.UrunAd.Contains(urun));
            }

            // Cari filtreleme
            if (!string.IsNullOrEmpty(cari))
            {
                satislar = satislar.Where(x =>
                    (x.Cariler.CariAd + " " + x.Cariler.CariSoyad).Contains(cari)
                );
            }

            // ViewBag'ler
            ViewBag.Urun = urun;
            ViewBag.Cari = cari;
            ViewBag.PageSize = pageSize;

            // PagedList
            var model = satislar
                .OrderByDescending(x => x.SatisID)
                .ToPagedList(page, pageSize);

            return View(model);
        }

    }
}