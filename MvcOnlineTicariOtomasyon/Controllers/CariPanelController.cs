using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcOnlineTicariOtomasyon.Models.Siniflar;

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
            var degerler = c.Carilers.FirstOrDefault(x => x.CariMail == mail.ToString());
            ViewBag.m = mail;
            return View(degerler);
        }
        public ActionResult Siparislerim()
        {
            var mail = (string)Session["CariMail"];
            var id = c.Carilers.Where(x => x.CariMail == mail).Select(y => y.CariID).FirstOrDefault();
            var degerler = c.satisHarekets.Where(x => x.CariID == id).ToList();
            return View(degerler);
        }

        // GELEN MESAJLAR
        public ActionResult GelenMesajlar()
        {
            string mail = (string)Session["CariMail"];

            var mesajlar = c.Mesajlars
                .Where(x => x.Alici == mail && x.Silindi == false)
                .OrderByDescending(x => x.Tarih)
                .ToList();

            return View(mesajlar);
        }

        public ActionResult GonderilenMesajlar()
        {
            string mail = (string)Session["CariMail"];

            var mesajlar = c.Mesajlars
                .Where(x => x.Gönderici == mail && x.Silindi == false)
                .OrderByDescending(x => x.Tarih)
                .ToList();

            return View(mesajlar);
        }
        public ActionResult CopKutusu()
        {
            string mail = (string)Session["CariMail"];

            var mesajlar = c.Mesajlars
                .Where(x => (x.Alici == mail || x.Gönderici == mail) && x.Silindi == true)
                .OrderByDescending(x => x.Tarih)
                .ToList();

            return View(mesajlar);
        }
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



        [HttpGet]
        public ActionResult YeniMesaj()
        {
            return View();
        }

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


    }
}