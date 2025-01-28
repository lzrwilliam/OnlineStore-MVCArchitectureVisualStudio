using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectPAW.ContextModels;
using ProiectPAW.wwwroot.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProiectPAWMvc.Controllers
{
    public class SearchController : Controller
    {
        private readonly ProdusContext _context;

        public SearchController(ProdusContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(new List<Produs>());
            }

            var produse = await _context.Produs
                .Where(p => p.Nume.Contains(query) || p.Descriere.Contains(query))
                .ToListAsync();

            return View(produse);
        }
    }
}
