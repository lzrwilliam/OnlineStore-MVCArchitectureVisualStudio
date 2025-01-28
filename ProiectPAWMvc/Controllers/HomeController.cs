using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectPAW.ContextModels;
using ProiectPAW.wwwroot.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProiectPAWMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProdusContext _context;

        public HomeController(ProdusContext context)
        {
            _context = context;
        }


        //!!! Maxim 5 de produse se afiseaza la ultimele vizualizate, se elimina apoi incepand cu cele mai vechi
        public async Task<IActionResult> Index()
        {
            // Cele mai vândute produse din fiecare categorie
            var mostSoldProductsByCategory = await _context.Categories
        .Where(c => c.ParentCategorieID == null)
        .Select(c => new
        {
            Category = c,
            TopProducts = c.Produse
                .Where(p => p.Comenzi.Any()) // verif daca exista comanda pt produs
                .SelectMany(p => p.Comenzi)
                .GroupBy(dc => dc.Produs)
                .OrderByDescending(g => g.Sum(dc => dc.Cantitate))
                .Select(g => g.Key)
                .Take(5) // primele 5 produse din fiecare categorie cu cele mai multe comenzi
        })
        .Where(c => c.TopProducts.Any()) // ne asiguram ca sunt produse din top
        .ToListAsync();

            var userId = User.Identity.IsAuthenticated ? int.Parse(User.FindFirst("IdUserLogat").Value) : (int?)null;
            var lastViewedProducts = new List<Produs>();

            if (userId.HasValue)
            {
                lastViewedProducts = await _context.ProdusVizualizat
                    .Where(pv => pv.UtilizatorID == userId)
                    .OrderByDescending(pv => pv.DataVizualizare)
                    .Select(pv => pv.Produs)
                    .Take(5)
                    .ToListAsync();
            }

            var viewModel = new HomeViewModel
            {
                MostSoldProductsByCategory = mostSoldProductsByCategory.Select(m => new
                {
                    m.Category,
                    m.TopProducts
                }).ToList<dynamic>(),
                LastViewedProducts = lastViewedProducts
            };

            return View(viewModel);
        }
    }

    public class HomeViewModel
    {
        public List<dynamic> MostSoldProductsByCategory { get; set; }
        public List<Produs> LastViewedProducts { get; set; }
    }

  
}
