using System;
using System.Linq;
using System.Web.Mvc;
using MvcOnlineTicariOtomasyon.Models.Siniflar;
using PagedList;
using System.Data.Entity;

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
        [AllowAnonymous]
        public ActionResult Dinamik()
        {
            Class4 cs = new Class4();
            cs.deger1 = c.Faturalars.Include(x => x.FaturaKalems).ToList();
            cs.deger2 = c.FaturaKalems.ToList();
            return View(cs);
        }
        [AllowAnonymous]
        [HttpPost] // Gelen veriyi kabul etmesi için şarttır
        public ActionResult FaturaKaydet(string FaturaSeriNo, string FaturaSıraNo, DateTime Tarih, string Saat, string VergiDairesi, string TeslimEden, string TeslimAlan, string Toplam, FaturaKalem[] kalemler)
        {
            // Yeni bir fatura oluştur
            Faturalar fatura = new Faturalar
            {
                FaturaSeriNo = FaturaSeriNo,
                FaturaSıraNo = FaturaSıraNo,
                VergiDairesi = VergiDairesi,
                Tarih = Tarih,
                Saat = Saat,
                TeslimEden = TeslimEden,
                TeslimAlan = TeslimAlan,
                // String gelen değeri Decimal'e (veya veritabanınızda neyse ona) çeviriyoruz
                Toplam = decimal.TryParse(Toplam, out decimal toplamValue) ? toplamValue : 0
            };

            // Faturayı veritabanına ekle
            c.Faturalars.Add(fatura);

            // ÖNCE faturayı kaydediyoruz ki, veritabanı faturaya bir ID versin (Identity Insert). 
            // Aksi halde FaturaID 0 olur ve kalemler boşta kalır veya hata verir.
            c.SaveChanges();

                foreach (var x in kalemler)
                {
                FaturaKalem faturaKalem = new FaturaKalem
                {
                    Aciklama = x.Aciklama,
                    Miktar = x.Miktar,
                    BirimFiyat = x.BirimFiyat,
                    Tutar = x.Tutar,
                    FaturaID = fatura.FaturaID // Yeni oluşturulan faturanın ID'sini kullan
                };
                c.FaturaKalems.Add(faturaKalem);
            }
                c.SaveChanges(); // Kalemleri kaydet
          

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
