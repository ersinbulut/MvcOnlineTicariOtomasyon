using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcOnlineTicariOtomasyon.Models.Siniflar;
using PagedList;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    public class KargoController : Controller
    {
        Context c = new Context();
        // GET: Kargo


        // 📄 LİSTELE + ARA + SAYFALA
        public ActionResult Index(string search, int page = 1, int pageSize = 10)
        {
            var kargolar = c.KargoDetays.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                kargolar = kargolar.Where(x =>
                    x.TakipKodu.Contains(search) ||
                    x.Alici.Contains(search) ||
                    x.Personel.Contains(search)
                );
            }

            ViewBag.CurrentSearch = search;
            ViewBag.PageSize = pageSize;

            var sonuc = kargolar
                .OrderByDescending(x => x.Tarih)
                .ToPagedList(page, pageSize);

            return View(sonuc);
        }

        [HttpGet]
        public ActionResult KargoEkle()
        {
            Random rnd = new Random();

            string[] harfler = { "A","B","C","D","E","F","G","H","I","J",
                         "K","L","M","N","O","P","R","S","T","U",
                         "V","Y","Z" };

            // 10 karakterlik takip kodu
            // Örnek: 4832A45B67
            string takipKodu =
                rnd.Next(1000, 9999).ToString() +              // 4
                harfler[rnd.Next(harfler.Length)] +             // 1
                rnd.Next(10, 99).ToString() +                   // 2
                harfler[rnd.Next(harfler.Length)] +             // 1
                rnd.Next(10, 99).ToString();                    // 2
                                                                // TOPLAM = 10

            ViewBag.takipkod = takipKodu;
            return View();
        }

        [HttpPost]
        public ActionResult KargoEkle(KargoDetay d)
        {
            c.KargoDetays.Add(d);
            c.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult KargoTakip(string id)
        {
            var kargodetay = c.KargoTakips
                              .Where(x => x.TakipKodu == id)
                              .OrderByDescending(x => x.TarihZaman)
                              .ToList();

            return View(kargodetay);
        }


    }
}