using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MvcOnlineTicariOtomasyon.Models.Siniflar;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    public class GrafikController : Controller
    {
        // GET: Grafik
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index2()
        {
            var grafikciz = new Chart(600, 600);
            grafikciz.AddTitle("Kategori - Ürün Stok Sayısı").AddLegend("Stok").
                AddSeries("Değerler", xValue: new[]
            { "Beyaz Eşya", "Bilgisayar", "Mobilya", "Telefon", "Küçük Ev Aletleri" },
                yValues: new[] { 85, 66, 98, 54, 23 }).Write();

            return File(grafikciz.ToWebImage().GetBytes(), "image/jpeg");
        }
        Context c = new Context();
        public ActionResult Index3()
        {
            ArrayList xvalue = new ArrayList();
            ArrayList yvalue = new ArrayList();
            var sonuclar = c.Uruns.ToList().Take(7);
            sonuclar.ToList().ForEach(x => xvalue.Add(x.UrunAd));
            sonuclar.ToList().ForEach(y => yvalue.Add(y.Stok));
            var grafikciz = new Chart(width: 800, height: 800);
            grafikciz.AddTitle("Ürün - Stok").AddLegend("Stok").
                AddSeries(chartType: "Pie", xValue: xvalue, yValues: yvalue).Write();
            return File(grafikciz.ToWebImage().GetBytes(), "image/jpeg");
        }

        public ActionResult Index4()
        {
            return View();
        }

        public ActionResult VisualizeUrunResult()
        {
            return Json(UrunListesi(), JsonRequestBehavior.AllowGet);
        }

        public List<sinif1> UrunListesi()
        {
            return new List<sinif1>
    {
        new sinif1 { urunad = "Bilgisayar", stok = 120 },
        new sinif1 { urunad = "Beyaz Eşya", stok = 150 },
        new sinif1 { urunad = "Mobilya", stok = 75 },
        new sinif1 { urunad = "Telefon", stok = 180 },
        new sinif1 { urunad = "Küçük Ev Aletleri", stok = 95 }
    };
        }


        public ActionResult Index5()
        {
            return View();
        }

        public ActionResult VisualizeUrunResult2()
        {
            return Json(UrunListesi2(), JsonRequestBehavior.AllowGet);
        }

        public List<sinif2> UrunListesi2()
        {
            using (var context = new Context())
            {
                return context.Uruns.Select(x => new sinif2
                {
                    urn = x.UrunAd,
                    stk = x.Stok
                }).Take(15).ToList();
            }
        }

        public ActionResult Index6()
        {
            return View();
        }

        public ActionResult Index7()
        {
            return View();
        }


    }
}