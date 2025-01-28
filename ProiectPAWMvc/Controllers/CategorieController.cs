using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectPAW.ContextModels;
using ProiectPAW.wwwroot.Models;
using ProiectPAWMvc.Models;
using X.PagedList;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class CategorieController : Controller
{
    private readonly ProdusContext _context;

    public CategorieController(ProdusContext context)
    {
        _context = context;
    }
    [Authorize(Policy = "ModeratorOnly")]
    public async Task<IActionResult> Index()
    {
        // Include atributele in query de categorii
        var categorii = await _context.Categories
                                      .Include(c => c.Atribute)
                                      .ToListAsync();
        return View(categorii);
    }

    public async Task<IActionResult> Edit(int id)
    {
        // la editare se includ atributele
        var categorie = await _context.Categories
                                      .Include(c => c.Atribute)
                                      .FirstOrDefaultAsync(c => c.ID == id);
        if (categorie == null)
        {
            return NotFound();
        }
        ViewBag.CategoriiParinte = new SelectList(_context.Categories.Where(c => c.ParentCategorieID == null && c.ID != id), "ID", "Nume", categorie.ParentCategorieID);
        return View(categorie);
    }
    [Authorize(Policy = "ModeratorOnly")]
    public IActionResult Create()
    {
        ViewBag.CategoriiParinte = new SelectList(_context.Categories.Where(c => c.ParentCategorieID == null), "ID", "Nume");
        return View();
    }
    [HttpPost]
    [Authorize(Policy = "ModeratorOnly")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Categorie categorie)
    {
        if (ModelState.IsValid)
        {
            _context.Add(categorie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewBag.CategoriiParinte = new SelectList(_context.Categories.Where(c => c.ParentCategorieID == null), "ID", "Nume", categorie.ParentCategorieID);
        return View(categorie);
    }

    [HttpPost]
    [Authorize(Policy = "ModeratorOnly")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAtribut(AtributCategorie atribut)
    {
        // Excludere validare pentru proprietatea categorie
        ModelState.Remove("Categorie");

        if (ModelState.IsValid)
        {
            Console.WriteLine("HEIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            _context.AtributCategorii.Add(atribut);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = atribut.CategorieID });
        }
        ViewBag.CategorieId = atribut.CategorieID;
        return View(atribut);
    }

   

    [HttpPost]
    [Authorize(Policy = "ModeratorOnly")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Categorie categorie)
    {
        if (id != categorie.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(categorie);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategorieExists(categorie.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewBag.CategoriiParinte = new SelectList(_context.Categories.Where(c => c.ParentCategorieID == null), "ID", "Nume", categorie.ParentCategorieID);
        return View(categorie);
    }

    [HttpGet]
    [Authorize(Policy = "ModeratorOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var categorie = await _context.Categories.FindAsync(id);
        if (categorie == null)
        {
            return NotFound();
        }
        return View(categorie);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "ModeratorOnly")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var categorie = await _context.Categories
       .Include(c => c.Subcategorii)  
       .FirstOrDefaultAsync(c => c.ID == id);

        await DeleteSubcategories(categorie.Subcategorii.ToList());

        _context.Categories.Remove(categorie);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    private async Task DeleteSubcategories(List<Categorie> subcategories)
    {
        foreach (var subcategory in subcategories)
        {
            // incarca subcategoriile categoriei curente
            var loadedSubcategories = await _context.Categories
                .Where(c => c.ParentCategorieID == subcategory.ID)
                .Include(c => c.Subcategorii)
                .ToListAsync();

            // sterge recursiv subcategoriile
            await DeleteSubcategories(loadedSubcategories);

            // sterge subcategoria
            _context.Categories.Remove(subcategory);
        }
    }
    private bool CategorieExists(int id)
    {
        return _context.Categories.Any(e => e.ID == id);
    }

    // Metode pentru administrarea atributelor
    [HttpGet]
    [Authorize(Policy = "ModeratorOnly")]
    public IActionResult CreateAtribut(int categorieId)
    {
        ViewBag.CategorieId = categorieId;
        return View(new AtributCategorie { CategorieID = categorieId });

    }


    [HttpGet]
    [Authorize(Policy = "ModeratorOnly")]
    public async Task<IActionResult> EditAtribut(int id)
    {
        var atribut = await _context.AtributCategorii.FindAsync(id);
        if (atribut == null)
        {
            return NotFound();
        }
        ViewBag.CategorieId = atribut.CategorieID;
        return View(atribut);
    }

    [HttpPost]
    [Authorize(Policy = "ModeratorOnly")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAtribut(int id, AtributCategorie atribut)
    {
        if (id != atribut.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(atribut);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AtributCategorieExists(atribut.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Edit), new { id = atribut.CategorieID });
        }
        ViewBag.CategorieId = atribut.CategorieID;
        return View(atribut);
    }


    [Authorize(Policy = "ModeratorOnly")]
    [HttpGet]
    public async Task<IActionResult> DeleteAtribut(int id)
    {
        var atribut = await _context.AtributCategorii.FindAsync(id);
        if (atribut == null)
        {
            return NotFound();
        }
        return View(atribut);
    }

    [HttpPost, ActionName("DeleteAtribut")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAtributConfirmed(int id)
    {
        var atribut = await _context.AtributCategorii.FindAsync(id);
        _context.AtributCategorii.Remove(atribut);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Edit), new { id = atribut.CategorieID });
    }

    private bool AtributCategorieExists(int id)
    {
        return _context.AtributCategorii.Any(e => e.ID == id);
    }

    [HttpGet]
    public async Task<IActionResult> Detalii(int id, string sortOrder, string searchString, int? pageNumber, decimal? minPrice, decimal? maxPrice, string brand)
    {
        Console.WriteLine("intra pe detali");
        var categorie = await _context.Categories
            .Include(c => c.Atribute)
            .Include(c => c.Subcategorii).ThenInclude(sc => sc.Atribute)
            .FirstOrDefaultAsync(c => c.ID == id);

        List<int> favoriteIds = new List<int>();

        if (categorie == null)
            return NotFound();

        var produse = _context.Produs
            .Include(p => p.ValoriAtribute)
            .ThenInclude(va => va.AtributCategorie)
            .Where(p => p.CategorieID == id || p.SubcategorieID == id);

        if (User.Identity.IsAuthenticated)
        {
            var userId = int.Parse(User.FindFirst("IdUserLogat").Value);
            favoriteIds = new List<int>(
                await _context.Favorite
                    .Where(f => f.UtilizatorID == userId)
                    .Select(f => f.ProdusID)
                    .ToListAsync()
            );
        }

        if (minPrice.HasValue)
            produse = produse.Where(p => p.Pret >= minPrice.Value);
        if (maxPrice.HasValue)
            produse = produse.Where(p => p.Pret <= maxPrice.Value);
        if (!string.IsNullOrEmpty(brand))
            produse = produse.Where(p => p.Brand.Contains(brand));

        foreach (var atribut in categorie.Atribute.Concat(categorie.Subcategorii.SelectMany(sc => sc.Atribute)))
        {
            var valueKey = $"atribut_{atribut.ID}";
            var valoareAtribut = Request.Query[valueKey].ToString();
            Console.WriteLine($"!!!!Debug: {valueKey} = {valoareAtribut}"); // Diagnostic output

            if (!string.IsNullOrEmpty(valoareAtribut))
            {
                switch (atribut.Tip)
                {
                    case "number":
                        string[] range = valoareAtribut.Split('-');
                        if (range.Length == 2 && decimal.TryParse(range[0], out decimal minVal) && decimal.TryParse(range[1], out decimal maxVal))
                        {
                            produse = produse.Where(p => p.ValoriAtribute.Any(va => va.AtributCategorieID == atribut.ID && decimal.Parse(va.Valoare) >= minVal && decimal.Parse(va.Valoare) <= maxVal));
                        }
                        break;
                    case "boolean":
                        bool isTrue = string.Equals(valoareAtribut, "true", StringComparison.OrdinalIgnoreCase);
                        produse = produse.Where(p => p.ValoriAtribute.Any(va => va.AtributCategorieID == atribut.ID && va.Valoare == (isTrue ? "true" : "false")));
                        break;
                    case "text":
                        produse = produse.Where(p => p.ValoriAtribute.Any(va => va.AtributCategorieID == atribut.ID && va.Valoare.Contains(valoareAtribut)));
                        break;
                }
            }
        }

        var pageSize = 10;
        var pagedList = await produse.ToPagedListAsync(pageNumber ?? 1, pageSize);

        var viewModel = new CategorieDetaliiViewModel
        {
            CategorieID = categorie.ID,
            CategorieNume = categorie.Nume,
            ParentCategorieID = categorie.ParentCategorieID,
            ParentCategorieNume = categorie.ParentCategorie?.Nume,
            Produse = pagedList,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Brand = brand,
            Atribute = categorie.Atribute.ToDictionary(a => a.ID, a => a.Nume),
            Subcategorii = categorie.Subcategorii.ToList(),

            AtributeDinamice = categorie.Atribute.Concat(categorie.Subcategorii.SelectMany(sc => sc.Atribute)).Select(a => new AtributDinamicFiltru
            {
                AtributCategorieID = a.ID,
                Nume = a.Nume,
                Tip = a.Tip
            }).ToList(),
            FavoriteIds = favoriteIds 

        };

        return View(viewModel);
    }






}
