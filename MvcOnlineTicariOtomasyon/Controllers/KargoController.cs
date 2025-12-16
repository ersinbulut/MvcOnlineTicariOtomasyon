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


        public ActionResult Index(string search, int page = 1, int pageSize = 10)
        {
            var kargolar = c.KargoDetays.AsQueryable();

            // 🔍 KARGO TAKİP NO + DİĞER ALANLAR
            if (!string.IsNullOrWhiteSpace(search))
            {
                kargolar = kargolar.Where(x =>
                    x.TakipKodu.Contains(search) ||   // ✅ KARGO TAKİP NO
                    x.Alici.Contains(search) ||
                    x.Personel.Contains(search)
                );
            }

            ViewBag.CurrentSearch = search;
            ViewBag.PageSize = pageSize;

            var pagedList = kargolar
                .OrderByDescending(x => x.Tarih)
                .ToPagedList(page, pageSize);

            return View(pagedList);
        }


    }
}