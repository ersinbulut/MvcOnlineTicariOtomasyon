using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcOnlineTicariOtomasyon.Models.Siniflar;
using PagedList;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    public class PersonelController : Controller
    {
        Context c = new Context();
        // GET: Personel
        public ActionResult Index(string search, int? department, int page = 1, int pageSize = 10)
        {
            var personeller = c.Personels.AsQueryable();

            // Arama
            if (!string.IsNullOrEmpty(search))
            {
                personeller = personeller.Where(x =>
                    x.PersonelAd.Contains(search) ||
                    x.PersonelSoyad.Contains(search));
            }

            // Departman filtreleme
            if (department != null)
            {
                personeller = personeller.Where(x => x.DepartmanID == department);
            }

            // Departman listesi
            ViewBag.Departmanlar = c.Departmans.ToList();

            ViewBag.Search = search;
            ViewBag.Department = department;
            ViewBag.PageSize = pageSize;

            return View(personeller
                        .OrderBy(x => x.PersonelID)
                        .ToPagedList(page, pageSize));
        }

        [HttpGet]
        public ActionResult PersonelEkle()
        {
            List<SelectListItem> deger1 = (from x in c.Departmans.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.DepartmanAd,
                                               Value = x.DepartmanID.ToString()
                                           }).ToList();
            ViewBag.dgr1 = deger1;
            return View();
        }
        [HttpPost]
        public ActionResult PersonelEkle(Personel p)
        {
            c.Personels.Add(p);
            c.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult PersonelGetir(int id)
        {
            List<SelectListItem> deger1 = (from x in c.Departmans.ToList()
                                           select new SelectListItem
                                           {
                                               Text = x.DepartmanAd,
                                               Value = x.DepartmanID.ToString()
                                           }).ToList();
            ViewBag.dgr1 = deger1;
            var prs = c.Personels.Find(id);
            return View("PersonelGetir",prs);
        }
        public ActionResult PersonelGuncelle(Personel p)
        {
            var prs = c.Personels.Find(p.PersonelID);
            prs.PersonelAd = p.PersonelAd;
            prs.PersonelSoyad = p.PersonelSoyad;
            prs.PersonelGorsel = p.PersonelGorsel;
            prs.DepartmanID = p.DepartmanID;
            c.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult PersonelListe()
        {
            var sorgu = c.Personels.ToList();
            return View(sorgu);
        }
    }
}