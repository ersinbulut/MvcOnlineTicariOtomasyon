using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Mvc;
using QRCoder;
using static QRCoder.QRCodeGenerator;

namespace MvcOnlineTicariOtomasyon.Controllers
{
    public class QRController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string kod, string tur)
        {
            if (string.IsNullOrEmpty(kod))
            {
                ViewBag.Hata = "Lütfen bir değer giriniz.";
                return View();
            }

            string qrText = "";

            switch (tur)
            {
                case "Urun":
                    qrText = "https://siteadresiniz.com/Urun/Detay/" + kod;
                    break;
                case "Cari":
                    qrText = "https://siteadresiniz.com/Cari/Detay/" + kod;
                    break;
                case "Fatura":
                    qrText = "https://siteadresiniz.com/Fatura/Detay/" + kod;
                    break;
                default:
                    qrText = kod;
                    break;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator generator = new QRCodeGenerator();
                QRCodeData data = generator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);

                QRCode qrCode = new QRCode(data);

                Bitmap qrBitmap = qrCode.GetGraphic(
                    20,
                    Color.Black,
                    Color.White,
                    null, // LOGO EKLEMEK İSTERSEN: Bitmap.FromFile(Server.MapPath("~/Content/logo.png"))
                    15,
                    6,
                    false
                );

                qrBitmap.Save(ms, ImageFormat.Png);
                ViewBag.karekodimage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                ViewBag.DownloadImage = Convert.ToBase64String(ms.ToArray());
            }

            return View();
        }

        public FileResult Download(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            return File(bytes, "image/png", "QRCode.png");
        }
    }
}
