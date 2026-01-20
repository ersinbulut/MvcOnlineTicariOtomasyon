using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MvcOnlineTicariOtomasyon.Models.Siniflar;
using PagedList;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    public class CariPanelController : Controller
    {
        Context c = new Context();
        // GET: CariPanel
        [Authorize]
        public ActionResult Index()
        {
            var mail = (string)Session["CariMail"];
            var degerler = c.Carilers.Where(x => x.CariMail == mail).ToList();
            ViewBag.m = mail;
            var mailid=c.Carilers.Where(x => x.CariMail == mail).Select(y => y.CariID).FirstOrDefault();
            ViewBag.mid = mailid;
            var toplamsatis = c.satisHarekets.Where(x => x.CariID == mailid).Count();
            ViewBag.toplamsatis = toplamsatis;
            var toplamtutar = c.satisHarekets.Where(x => x.CariID == mailid).Sum(y => y.ToplamTutar);
            ViewBag.toplamtutar = toplamtutar;
            var toplamurunsayisi = c.satisHarekets.Where(x => x.CariID == mailid).Sum(y=> y.Adet);
            ViewBag.toplamurunsayisi = toplamurunsayisi;
            return View(degerler);
        }
        [Authorize]
        public ActionResult Siparislerim()
        {
            var mail = (string)Session["CariMail"];
            var id = c.Carilers.Where(x => x.CariMail == mail).Select(y => y.CariID).FirstOrDefault();
            var degerler = c.satisHarekets.Where(x => x.CariID == id).ToList();
            return View(degerler);
        }
        [Authorize]
        // GELEN MESAJLAR
        public ActionResult GelenMesajlar()
        {
            string mail = (string)Session["CariMail"];

            var mesajlar = c.Mesajlars
                .Where(x => x.Alici == mail && x.Silindi == false)
                .OrderByDescending(x => x.Tarih)
                .ToList();
            var gelensayisi = c.Mesajlars.Count(x => x.Alici == mail).ToString();
            ViewBag.d1 = gelensayisi;

            var gidensayisi = c.Mesajlars.Count(x => x.Alici == mail).ToString();
            ViewBag.d2 = gidensayisi;

            return View(mesajlar);
        }
        [Authorize]
        public ActionResult GonderilenMesajlar()
        {
            string mail = (string)Session["CariMail"];

            var mesajlar = c.Mesajlars
                .Where(x => x.Gönderici == mail && x.Silindi == false)
                .OrderByDescending(x => x.Tarih)
                .ToList();

            return View(mesajlar);
        }
        [Authorize]
        public ActionResult CopKutusu()
        {
            string mail = (string)Session["CariMail"];

            var mesajlar = c.Mesajlars
                .Where(x => (x.Alici == mail || x.Gönderici == mail) && x.Silindi == true)
                .OrderByDescending(x => x.Tarih)
                .ToList();

            return View(mesajlar);
        }
        [Authorize]
        public ActionResult GeriAl(int id)
        {
            var mesaj = c.Mesajlars.Find(id);

            if (mesaj.Alici == Session["CariMail"].ToString() ||
                mesaj.Gönderici == Session["CariMail"].ToString())
            {
                mesaj.Silindi = false;
                c.SaveChanges();
            }

            return RedirectToAction("CopKutusu");
        }

        [Authorize]
        // MESAJ DETAY
        public ActionResult MesajDetay(int id)
        {
            var mesaj = c.Mesajlars.Find(id);

            if (mesaj.Alici == Session["CariMail"].ToString())
            {
                mesaj.Okundu = true;
                c.SaveChanges();
            }

            return View(mesaj);
        }
        [Authorize]
        [HttpGet]
        public ActionResult Yanıtla(int id)
        {
            var eskiMesaj = c.Mesajlars.Find(id);

            Mesajlar m = new Mesajlar
            {
                Alici = eskiMesaj.Gönderici,
                Konu = "RE: " + eskiMesaj.Konu
            };

            return View(m);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Yanıtla(Mesajlar m)
        {
            m.Gönderici = (string)Session["CariMail"];
            m.Tarih = DateTime.Now;
            m.Okundu = false;
            m.Silindi = false;

            c.Mesajlars.Add(m);
            c.SaveChanges();

            return RedirectToAction("GonderilenMesajlar");
        }

        [Authorize]
        public ActionResult MesajSil(int id)
        {
            var mesaj = c.Mesajlars.Find(id);

            if (mesaj.Alici == Session["CariMail"].ToString() ||
                mesaj.Gönderici == Session["CariMail"].ToString())
            {
                mesaj.Silindi = true;
                c.SaveChanges();
            }

            return RedirectToAction("GelenMesajlar");
        }


        [Authorize]
        [HttpGet]
        public ActionResult YeniMesaj()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult YeniMesaj(Mesajlar m)
        {
            m.Gönderici = (string)Session["CariMail"];
            m.Tarih = DateTime.Now;
            m.Okundu = false;
            m.Silindi = false;

            c.Mesajlars.Add(m);
            c.SaveChanges();

            return RedirectToAction("GelenMesajlar");
        }
        [Authorize]
        public ActionResult KargoTakip(string search, int page = 1, int pageSize = 10)
        {
            var kargolar = c.KargoDetays.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                kargolar = kargolar.Where(x => x.TakipKodu == search);
            }

            ViewBag.CurrentSearch = search;
            ViewBag.PageSize = pageSize;

            var sonuc = kargolar
                .OrderByDescending(x => x.Tarih)
                .ToPagedList(page, pageSize);

            return View(sonuc);
        }
        [Authorize]
        public ActionResult CariKargoTakip(string id)
        {
            var kargo = c.KargoTakips.Where(x => x.TakipKodu == id).ToList();
            return View(kargo);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }
    }
}