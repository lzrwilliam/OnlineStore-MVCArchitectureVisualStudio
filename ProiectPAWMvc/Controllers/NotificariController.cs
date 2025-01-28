using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ProiectPAW.ContextModels;
using ProiectPAWMvc.Models;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace ProiectPAWMvc.Controllers
{
    [Authorize]
    public class NotificariController : Controller
    {
        private readonly ProdusContext _context;

        public NotificariController(ProdusContext context)
        {
            _context = context;
        }

      
        

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst("IdUserLogat").Value);
         //   ViewBag.NotificariNecitite = _context.Notificari.Count(n => n.UtilizatorID == userId && !n.Citita);

           // Adaugam o linie în NotificariController
           // pentru a ne asigura că ViewBag.NotificariNecitite este populat in orice actiune:


            var notificari = await _context.Notificari
                .Where(n => n.UtilizatorID == userId)
                .Include(n => n.Produs)
                .ToListAsync();

            //  ViewBag.NotificariNecitite = notificari.Count(n => !n.Citita);

            var notificariNecitite = notificari.Count(n => !n.Citita);

            ViewData["NotificariNecitite"] = notificariNecitite;

            return View(notificari);
        }

        [HttpPost]
        public async Task<IActionResult> MarcheazaCaCitit(int id)
        {
            var notificare = await _context.Notificari.FindAsync(id);

            if (notificare != null)
            {
                if (notificare.Citita is true)
                {
                    TempData["ErrorMessage"] = "Notificarea este deja citita!";
                }
                else
                {
                    notificare.Citita = true;

                    _context.Update(notificare);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> StergeNotificare(int id)
        {
            var notificare = await _context.Notificari.FindAsync(id);

            if (notificare != null)
            {
                _context.Notificari.Remove(notificare);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> NumarNotificariNecitite()
        {
            var userId = int.Parse(User.FindFirst("IdUserLogat").Value);
            var numarNotificari = await _context.Notificari
                .CountAsync(n => n.UtilizatorID == userId && !n.Citita);
            return Json(numarNotificari);
        }

    }
}
