using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcOnlineTicariOtomasyon.Models.Siniflar;
using PagedList;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    public class SatisController : Controller
    {
        Context c = new Context();
        // GET: Satis
        public ActionResult Index(string urun, string personel, int page = 1, int pageSize = 10)
        {
            var satislar = c.satisHarekets.AsQueryable();

            // 🔍 Ürün Arama
            if (!string.IsNullOrEmpty(urun))
            {
                satislar = satislar.Where(x => x.Urun.UrunAd.Contains(urun));
            }

            // 👤 Personel Filtresi
            if (!string.IsNullOrEmpty(personel))
            {
                satislar = satislar.Where(x =>
                    (x.Personel.PersonelAd + " " + x.Personel.PersonelSoyad) == personel
                );
            }

            // 📌 Personel Listesi (dropdown için)
            var personelList = c.Personels
                .Select(x => x.PersonelAd + " " + x.PersonelSoyad)
                .Distinct()
                .ToList();

            ViewBag.Personeller = personelList;

            // 📌 ViewBag'lere filtre bilgilerini gönder
            ViewBag.Urun = urun;
            ViewBag.SelectedPersonel = personel;
            ViewBag.PageSize = pageSize;

            // 📄 Sayfalama
            var model = satislar
                .OrderByDescending(x => x.SatisID)
                .ToPagedList(page, pageSize);

            return View(model);
        }

        [HttpGet]
        public ActionResult YeniSatis()
        {
            List<SelectListItem> deger1 = (from x in c.Uruns.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.UrunAd,
                                               Value = x.UrunID.ToString()
                                           }).ToList();

            List<SelectListItem> deger2 = (from x in c.Carilers.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.CariAd+ " "+ x.CariSoyad,
                                               Value = x.CariID.ToString()
                                           }).ToList();

            List<SelectListItem> deger3 = (from x in c.Personels.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.PersonelAd + " " + x.PersonelSoyad,
                                               Value = x.PersonelID.ToString()
                                           }).ToList();
            ViewBag.dgr1 = deger1;
            ViewBag.dgr2 = deger2;
            ViewBag.dgr3 = deger3;
            return View();
        }
        [HttpPost]
        public ActionResult YeniSatis(SatisHareket s)
        {
            s.Tarih = DateTime.Parse(DateTime.Now.ToShortDateString());
            c.satisHarekets.Add(s);
            c.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SatisGetir(int id)
        {
            List<SelectListItem> deger1 = (from x in c.Uruns.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.UrunAd,
                                               Value = x.UrunID.ToString()
                                           }).ToList();

            List<SelectListItem> deger2 = (from x in c.Carilers.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.CariAd + " " + x.CariSoyad,
                                               Value = x.CariID.ToString()
                                           }).ToList();

            List<SelectListItem> deger3 = (from x in c.Personels.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.PersonelAd + " " + x.PersonelSoyad,
                                               Value = x.PersonelID.ToString()
                                           }).ToList();
            ViewBag.dgr1 = deger1;
            ViewBag.dgr2 = deger2;
            ViewBag.dgr3 = deger3;
            var deger = c.satisHarekets.Find(id);
            return View("SatisGetir", deger);
        }

        public ActionResult SatisGuncelle(SatisHareket p)
        {
            var deger = c.satisHarekets.Find(p.SatisID);
            deger.CariID = p.CariID;
            deger.Adet = p.Adet;
            deger.Fiyat = p.Fiyat;
            deger.PersonelID = p.PersonelID;
            deger.Tarih = p.Tarih;
            deger.ToplamTutar = p.ToplamTutar;
            deger.UrunID = p.UrunID;
            c.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SatisDetay(int id)
        {
            var degerler = c.satisHarekets.Where(x => x.SatisID == id).ToList();
            return View(degerler);
        }

    }
}