using System;
using System.Linq;
using System.Web.Mvc;
using MvcOnlineTicariOtomasyon.Models.Siniflar;
using PagedList;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    public class FaturaController : Controller
    {
        private Context c = new Context();

        // GET: Fatura Listesi
        public ActionResult Index(string search, string vergiDairesi, int page = 1, int pageSize = 10)
        {
            var liste = c.Faturalars.AsQueryable();

            // Arama filtresi
            if (!string.IsNullOrEmpty(search))
            {
                liste = liste.Where(x => x.FaturaSeriNo.Contains(search) || x.FaturaSıraNo.Contains(search));
            }

            // Vergi Dairesi filtresi
            if (!string.IsNullOrEmpty(vergiDairesi))
            {
                liste = liste.Where(x => x.VergiDairesi == vergiDairesi);
            }

            liste = liste.OrderBy(x => x.FaturaID);

            var paged = liste.ToPagedList(page, pageSize);

            ViewBag.Search = search;
            ViewBag.VergiDaireleri = c.Faturalars.Select(x => x.VergiDairesi).Distinct().ToList();
            ViewBag.SelectedVergiDairesi = vergiDairesi;
            ViewBag.PageSize = pageSize;

            return View(paged);
        }

        // GET: Fatura Ekle
        [HttpGet]
        public ActionResult FaturaEkle()
        {
            return View();
        }

        // POST: Fatura Ekle
        [HttpPost]
        public ActionResult FaturaEkle(Faturalar f)
        {
            c.Faturalars.Add(f);
            c.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Fatura Güncelle
        public ActionResult FaturaGetir(int id)
        {
            var fatura = c.Faturalars.Find(id);
            return View(fatura);
        }

        [HttpPost]
        public ActionResult FaturaGuncelle(Faturalar f)
        {
            var fatura = c.Faturalars.Find(f.FaturaID);
            if (fatura != null)
            {
                fatura.FaturaSeriNo = f.FaturaSeriNo;
                fatura.FaturaSıraNo = f.FaturaSıraNo;
                fatura.Saat = f.Saat;
                fatura.Tarih = f.Tarih;
                fatura.TeslimAlan = f.TeslimAlan;
                fatura.TeslimEden = f.TeslimEden;
                fatura.VergiDairesi = f.VergiDairesi;
                c.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Fatura Detay
        public ActionResult FaturaDetay(int id)
        {
            var degerler = c.FaturaKalems.Where(x => x.FaturaID == id).ToList();
            return View(degerler);
        }

        // 🔹 Popup: Fatura Kalemleri
        // FaturaController.cs içinde:
        // 🔹 Popup: Fatura Kalemleri
        [HttpGet]
        public ActionResult FaturaKalemPopup(int id)
        {
            try
            {
                var kalemler = c.FaturaKalems.Where(x => x.FaturaID == id).ToList();
                ViewBag.FaturaID = id;
                // DÜZELTME: Partial View'un dosya adı ile eşleştirildi
                return PartialView("faturakalempopup", kalemler);
            }
            catch (Exception ex)
            {
                return Content("Hata: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        // 🔹 Popup: Yeni Kalem GET
        [HttpGet]
        public ActionResult YeniKalemPopup(int faturaId)
        {
            try
            {
                ViewBag.FaturaID = faturaId;
                // DÜZELTME: Partial View'un dosya adı ile eşleştirildi
                return PartialView("yenikalempopup");
            }
            catch (Exception ex)
            {
                return Content("Hata: " + ex.Message + "\n" + ex.StackTrace);
            }
        }

        // 🔹 Popup: Yeni Kalem POST
        [HttpPost]
        public ActionResult YeniKalemPopup(FaturaKalem f)
        {
            try
            {
                c.FaturaKalems.Add(f);
                c.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
