using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectPAW.ContextModels;
using System.Linq;
using System.Threading.Tasks;

namespace ProiectPAWMvc.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly ProdusContext _context;

        public FavoriteController(ProdusContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst("IdUserLogat").Value);

            var favoriteProduse = await _context.Favorite
                .Where(f => f.UtilizatorID == userId)
                .Include(f => f.Produs)
                    .ThenInclude(p => p.Categorie)
                .Include(f => f.Produs)
                    .ThenInclude(p => p.Subcategorie)
                .Select(f => f.Produs)
                .ToListAsync();

            return View(favoriteProduse);
        }

        [HttpPost]
        public async Task<IActionResult> EliminaDinFavorite(int produsId)
        {
            var userId = int.Parse(User.FindFirst("IdUserLogat").Value);

            var favorite = await _context.Favorite.FirstOrDefaultAsync(f => f.ProdusID == produsId && f.UtilizatorID == userId);
            if (favorite != null)
            {
                _context.Favorite.Remove(favorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
