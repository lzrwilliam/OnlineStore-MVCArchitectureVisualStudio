using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectPAW.ContextModels;
using System.Security.Claims;

namespace ProiectPAWMvc.Controllers;

public class ComenziUserController : Controller
{
    private readonly ProdusContext _context;

    public ComenziUserController(ProdusContext context)
    {
        _context = context;
    }

    // lista de comenzi pentru utilizatorul curent
    public async Task<IActionResult> Index()
    {
        // Preia ID-ul utilizatorului logat din Claim
        if (!User.Identity.IsAuthenticated)
        {
            return Redirect("/authentication/login"); 
        }

        var userClaim = User.FindFirst("IdUserLogat");
        if (userClaim == null)
        {
            return BadRequest("Sesiunea dumneavoastra a expirat. Va rugam sa va  logati din nou!");
        }

        if (!int.TryParse(userClaim.Value, out int userId))
        {
            return BadRequest("Eroare la preluarea informatiilor utilizatorului.");
        }

        var comenzi = await _context.Comenzi
                                    .Where(c => c.UtilizatorID == userId)
                                    .Include(c => c.DetaliiComenzi)
                                        .ThenInclude(dc => dc.Produs)
                                    .ToListAsync();

        return View(comenzi);
    }

    // detalii unei comenzi anume
    public async Task<IActionResult> Detalii(int id)
    {
        var comanda = await _context.Comenzi
                                    .Include(c => c.DetaliiComenzi)
                                        .ThenInclude(dc => dc.Produs)
                                    .FirstOrDefaultAsync(c => c.ID == id);

        if (comanda == null)
        {
            return NotFound("Comanda nu a fost gasita.");
        }

        return View(comanda);
    }
}
