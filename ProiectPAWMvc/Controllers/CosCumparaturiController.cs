using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProiectPAW.ContextModels;
using ProiectPAW.Models;
using ProiectPAWMvc.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProiectPAWMvc.Controllers
{
    [Authorize]
    public class CosCumparaturiController : Controller
    {
        private readonly ProdusContext _context;

        public CosCumparaturiController(ProdusContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var userId = int.Parse(User.FindFirst("IdUserLogat").Value);
            var cosItems = await _context.CosCumparaturi
                .Include(c => c.Produs)
                .Where(c => c.UtilizatorID == userId)
                .ToListAsync();

            return View(cosItems);
        }

        public async Task<IActionResult> AdaugaInCos(int produsId, int cantitate)
        {
            var userId = int.Parse(User.FindFirst("IdUserLogat").Value);

            var produs = await _context.Produs.FindAsync(produsId);

            if (produs == null || produs.Stoc < cantitate)
            {
                return NotFound();
            }

            var existingItem = await _context.CosCumparaturi
                .FirstOrDefaultAsync(c => c.ProdusID == produsId && c.UtilizatorID == userId);

            if (existingItem != null)
            {
                if (existingItem.Cantitate + cantitate <= produs.Stoc)
                {
                    existingItem.Cantitate += cantitate;
                }
                else
                {
                    TempData["ErrorMessage"] = "Cantitatea dorită depășește stocul disponibil.";
                    return RedirectToAction("Detalii", "Produs", new { prodID = produsId });
                }
            }
            else
            {
                if (cantitate <= produs.Stoc)
                {
                    var cosItem = new CosCumparaturi
                    {
                        UtilizatorID = userId,
                        ProdusID = produsId,
                        Cantitate = cantitate
                    };

                    _context.CosCumparaturi.Add(cosItem);
                }
                else
                {
                    TempData["ErrorMessage"] = "Cantitatea dorită depășește stocul disponibil.";
                    return RedirectToAction("Detalii", "Produs", new { prodID = produsId });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> StergeDinCos(int id)
        {
            var cosItem = await _context.CosCumparaturi.FindAsync(id);
            if (cosItem == null)
            {
                return NotFound();
            }

            _context.CosCumparaturi.Remove(cosItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plateste()
        {
            decimal totalComanda = 0;

            var userId = int.Parse(User.FindFirst("IdUserLogat").Value);
            var cosItems = await _context.CosCumparaturi
                .Include(c => c.Produs)
                .Where(c => c.UtilizatorID == userId)
                .ToListAsync();
            if (cosItems.IsNullOrEmpty())
            {
                TempData["ErrorMessage"] = "Cosul este gol!";
                return RedirectToAction(nameof(Index));


            }

            foreach (var item in cosItems)
            {
                var produs = await _context.Produs.FindAsync(item.ProdusID);
                if (produs != null && produs.Stoc >= item.Cantitate)
                {
                    totalComanda += item.Cantitate * produs.Pret;
                    

                }
                else
                {
                    TempData["ErrorMessage"] = $"Produsul {item.Produs.Nume} nu mai este disponibil in cantitatea dorita.";
                    return RedirectToAction(nameof(Index));

                }
            }

            foreach (var item in cosItems)
            {
                item.Produs.Stoc -= item.Cantitate; // actualizare stoc
            }

            var comanda = new Comanda
            {
                UtilizatorID = userId,
                DataComanda = DateTime.Now,
                TotalComanda = totalComanda,
                DetaliiComenzi = cosItems.Select(item => new DetaliiComanda
                {
                    ProdusID = item.ProdusID,
                    Cantitate = item.Cantitate,
                    PretUnitate = item.Produs.Pret,
                    NumeProdus = item.Produs.Nume

                }).ToList()
            };

            _context.Comenzi.Add(comanda);
            _context.CosCumparaturi.RemoveRange(cosItems); // sterge toate elementele din cos dupa plasarea comenzii


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> IncrementaCantitate(int id)
        {
            var cosItem = await _context.CosCumparaturi
                .Include(c => c.Produs)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (cosItem != null && cosItem.Cantitate < cosItem.Produs.Stoc)
            {
                cosItem.Cantitate++;
                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["ErrorMessage"] = "Cantitatea dorita depaseste stocul disponibil.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DecrementaCantitate(int id)
        {
            var cosItem = await _context.CosCumparaturi.FindAsync(id);

            if (cosItem != null && cosItem.Cantitate > 1)
            {
                cosItem.Cantitate--;
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.CosCumparaturi.Remove(cosItem);
                await _context.SaveChangesAsync();
                TempData["ErrorMessage"] = "Produs eliminat din cos.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
