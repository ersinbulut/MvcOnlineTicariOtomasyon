using MvcOnlineTicariOtomasyon.Models.Siniflar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    public class İstatistikController : Controller
    {
        Context c = new Context();
        // GET: İstatistik
        public ActionResult Index()
        {
            try
            {
                // Toplam kayıt sayıları
                ViewBag.d1 = c.Carilers?.Count().ToString() ?? "0";
                ViewBag.d2 = c.Uruns?.Count().ToString() ?? "0";
                ViewBag.d3 = c.Personels?.Count().ToString() ?? "0";
                ViewBag.d4 = c.Kategoris?.Count().ToString() ?? "0";

                // Stok toplamı
                ViewBag.d5 = c.Uruns?.Sum(x => (int?)x.Stok)?.ToString() ?? "0";

                // Marka sayısı
                ViewBag.d6 = c.Uruns?.Select(x => x.Marka).Distinct().Count().ToString() ?? "0";

                // Stokta 20 veya altı ürün sayısı
                ViewBag.d7 = c.Uruns?.Count(x => x.Stok <= 20).ToString() ?? "0";

                // En pahalı ve en ucuz ürün
                ViewBag.d8 = c.Uruns?.OrderByDescending(x => x.SatisFiyat).Select(x => x.UrunAd).FirstOrDefault() ?? "-";
                ViewBag.d9 = c.Uruns?.OrderBy(x => x.SatisFiyat).Select(x => x.UrunAd).FirstOrDefault() ?? "-";

                // Belirli ürünlerin sayısı
                ViewBag.d10 = c.Uruns?.Count(x => x.UrunAd == "Buzdolabı").ToString() ?? "0";
                ViewBag.d11 = c.Uruns?.Count(x => x.UrunAd == "Laptop").ToString() ?? "0";

                // En çok ürünü olan marka
                ViewBag.d12 = c.Uruns?.GroupBy(x => x.Marka)
                                     .OrderByDescending(z => z.Count())
                                     .Select(y => y.Key)
                                     .FirstOrDefault() ?? "-";

                // En çok satılan ürün
                var enCokSatanUrunID = c.satisHarekets?
                                         .GroupBy(u => u.UrunID)
                                         .OrderByDescending(g => g.Count())
                                         .Select(g => g.Key)
                                         .FirstOrDefault();

                ViewBag.d13 = c.Uruns?.Where(x => x.UrunID == enCokSatanUrunID)
                                       .Select(x => x.UrunAd)
                                       .FirstOrDefault() ?? "-";

                // Toplam satış
                ViewBag.d14 = c.satisHarekets?.Sum(x => (decimal?)x.ToplamTutar)?.ToString() ?? "0";

                // Bugünün satışları
                DateTime bugun = DateTime.Today;

                if (c.satisHarekets != null && c.satisHarekets.Any(x => x.Tarih == bugun))
                {
                    ViewBag.d15 = c.satisHarekets.Count(x => x.Tarih == bugun).ToString();
                    ViewBag.d16 = c.satisHarekets.Where(x => x.Tarih == bugun)
                                                 .Sum(x => (decimal?)x.ToplamTutar)
                                                 .ToString();
                }
                else
                {
                    ViewBag.d15 = "0";
                    ViewBag.d16 = "0";
                }
            }
            catch (Exception ex)
            {
                // Hata oluşursa tüm ViewBag değerlerini default ata ve mesaj göster
                ViewBag.Hata = "Veriler alınırken bir hata oluştu: " + ex.Message;

                ViewBag.d1 = ViewBag.d2 = ViewBag.d3 = ViewBag.d4 = "0";
                ViewBag.d5 = ViewBag.d6 = ViewBag.d7 = ViewBag.d8 = ViewBag.d9 = "0";
                ViewBag.d10 = ViewBag.d11 = ViewBag.d12 = ViewBag.d13 = "0";
                ViewBag.d14 = ViewBag.d15 = ViewBag.d16 = "0";
            }

            return View();
        }


        public ActionResult KolayTablolar()
        {
            var sorgu = from x in c.Carilers
                        group x by x.CariSehir into g
                        select new SinifGrup
                        {
                            Sehir = g.Key,
                            Sayi = g.Count()
                        };

            return View(sorgu.ToList());
        }
        public PartialViewResult Partial1()
        {
            var sorgu2 = from x in c.Personels
                         group x by x.Departman.DepartmanAd into g
                         select new SinifGrup2
                         {
                             Departman = g.Key,
                             Sayi = g.Count()
                         };
            return PartialView(sorgu2.ToList());
        }
        public PartialViewResult Partial2()
        {
            var sorgu3 = c.Carilers.ToList();
            return PartialView(sorgu3);
        }
        public PartialViewResult Partial3()
        {
            var sorgu4 = c.Uruns.ToList();
            return PartialView(sorgu4);
        }
        public PartialViewResult Partial4()
        {
            var sorgu5 = from x in c.Uruns
                         group x by x.Marka into g
                         select new SinifGrup3
                         {
                             marka = g.Key,
                             sayi = g.Count()
                         };
            return PartialView(sorgu5.ToList());
        }
    }
}