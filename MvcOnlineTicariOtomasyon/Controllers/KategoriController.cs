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
        [AllowAnonymous]
        public ActionResult Deneme()
        {
            Class3 cs = new Class3();

            // Kategorileri yükle
            cs.Kategoriler = new SelectList(c.Kategoris.ToList(), "KategoriID", "KategoriAd");

            // İlk açılışta ürünler tablosunun boş gelmesi daha mantıklı olabilir. 
            // Tüm ürünleri yüklemek isterseniz eski kodunuzu (c.Uruns.ToList()) kullanabilirsiniz.
            cs.Urunler = new SelectList(new List<Urun>(), "UrunID", "UrunAd");

            return View(cs);
        }

        [HttpPost] // Sadece POST isteklerini kabul et
        [AllowAnonymous]
        public JsonResult UrunGetir(int p)
        {
            // Join kullanmanıza hiç gerek yok, KategoriID üzerinden direkt filtreleyebilirsiniz.
            // Not: Modelinizdeki kolon adı 'KategoriID' yerine 'KategoriId' ise onu düzeltin.
            var urunlistesi = c.Uruns
                               .Where(x => x.Kategoriid == p)
                               .Select(x => new
                               {
                                   Text = x.UrunAd,
                                   Value = x.UrunID.ToString()
                               }).ToList();

            return Json(urunlistesi, JsonRequestBehavior.AllowGet);
        }
    }
}