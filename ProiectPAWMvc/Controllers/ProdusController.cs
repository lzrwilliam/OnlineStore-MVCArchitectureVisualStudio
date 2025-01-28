 using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProiectPAW.ContextModels;
using ProiectPAW.wwwroot.Models;
using ProiectPAWMvc.Models;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using ProiectPAW.Models;
using System.Net.Mail;
using System.Net;

namespace ProiectPAWMvc.Controllers
{
    public class ProdusController : Controller
    {
        private readonly ProdusContext _context;
        private readonly string defaultImage = "ProdNoPicture.png"; // poza default pt prod fara poza

        public ProdusController(ProdusContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Detalii(int prodID)
        {
            var userId = User.Identity.IsAuthenticated ? int.Parse(User.FindFirst("IdUserLogat").Value) : (int?)null;

            var produs = await _context.Produs
     .Include(p => p.Categorie)
         .ThenInclude(c => c.Atribute)
     .Include(p => p.Subcategorie)
         .ThenInclude(sc => sc.Atribute)
     .Include(p => p.ValoriAtribute)
         .ThenInclude(va => va.AtributCategorie)
     .Include(p => p.Recenzii)
         .ThenInclude(r => r.Utilizator)
     .Include(p => p.Favorite)
     .FirstOrDefaultAsync(p => p.ID == prodID);

            if (produs == null)
            {
                return NotFound();
            }

            if (userId.HasValue)
            {
                await AdaugaProdusVizualizat(userId.Value, prodID);
            } // Limita de 20 produse ultimele vizualizate, dupa se face suprascriere de la cele mai vechi spre cele mai noi

            ViewBag.HasPurchased = userId.HasValue && await _context.Comenzi
                .Include(c => c.DetaliiComenzi)
                .AnyAsync(c => c.UtilizatorID == userId && c.DetaliiComenzi.Any(dc => dc.ProdusID == prodID));

            var userReview = produs.Recenzii.FirstOrDefault(r => r.UtilizatorID == userId);
            ViewBag.HasReviewed = userReview != null;
            ViewBag.UserReview = userReview;
            ViewBag.UserID = userId;

            return View(produs);
        }

        private async Task AdaugaProdusVizualizat(int userId, int produsId)
        {
            var produsVizualizatExistent = await _context.ProdusVizualizat
                .FirstOrDefaultAsync(pv => pv.UtilizatorID == userId && pv.ProdusID == produsId);

            if (produsVizualizatExistent != null)
            {
                // Actualizeaza data vizualizarii daca produsul a fost deja vizualizat
                produsVizualizatExistent.DataVizualizare = DateTime.Now;
            }
            else
            {
                // Verifica daca lista de produse vizualizate a atins limita de 20 si elimina cel mai vechi daca da
                var produseVizualizate = await _context.ProdusVizualizat
                    .Where(pv => pv.UtilizatorID == userId)
                    .OrderByDescending(pv => pv.DataVizualizare)
                    .ToListAsync();

                if (produseVizualizate.Count >= 20)
                {
                    _context.ProdusVizualizat.Remove(produseVizualizate.Last());
                }

                // Adauga noul produs vizualizat
                var nouProdusVizualizat = new ProdusVizualizat
                {
                    UtilizatorID = userId,
                    ProdusID = produsId,
                    DataVizualizare = DateTime.Now
                };

                _context.ProdusVizualizat.Add(nouProdusVizualizat);
            }

            await _context.SaveChangesAsync();
        }


        [HttpGet]
        [Authorize(Policy = "ModeratorOnly")]
        public async Task<IActionResult> AdaugaProdus()
        {
            var categorii = await _context.Categories.Where(c => c.ParentCategorieID == null).ToListAsync();
            ViewBag.Categorii = new SelectList(categorii, "ID", "Nume");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdaugaProdus(Produs produs, IFormFile? Poza)
        {
            if (ModelState.IsValid)
            {
                if (Poza != null && Poza.Length > 0)
                {
                    // Crează numele fisierului din numele produsului, id si extensia  pozei
                    var sanitizedProductName = string.Concat(produs.Nume.Where(c => !Path.GetInvalidFileNameChars().Contains(c))).Replace(" ", "_");
                    var fileName = $"{sanitizedProductName}_{produs.ID}_{Path.GetRandomFileName()}{Path.GetExtension(Poza.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "src", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Poza.CopyToAsync(stream);
                    }
                    produs.Poza = fileName;
                }
                else
                {
                    produs.Poza = defaultImage;
                }
                _context.Produs.Add(produs);
                await _context.SaveChangesAsync();

                // Salvarea atributelor dinamice
                SaveDynamicAttributes(produs.ID, Request.Form);
                TempData["SuccessMessage"] = "Produsul adaugat cu success!";

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); 
            }

            // repopulam lista de categorii si subcategorii pentru View
            ViewBag.Categorii = new SelectList(await _context.Categories.ToListAsync(), "ID", "Nume", produs.CategorieID);
            return View(produs);
        }

        private void SaveDynamicAttributes(int produsId, IFormCollection form)
        {
            foreach (var key in form.Keys.Where(k => k.StartsWith("Atribut_")))
            {
                var splitKey = key.Split('_');
                if (splitKey.Length == 2 && int.TryParse(splitKey[1], out int attributeId))
                {
                    var value = form[key].ToString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        var valoareAtribut = new ValoareAtribut
                        {
                            ProdusID = produsId,
                            AtributCategorieID = attributeId,
                            Valoare = value
                        };
                        _context.ValoareAtribute.Add(valoareAtribut);
                    }
                }
            }
            _context.SaveChanges();
        }


        [HttpGet]
        [Authorize(Policy = "ModeratorOnly")]
        public async Task<IActionResult> ListaProduse(string actionType) // pentru listarea produselor la actiunile de moderator
        {
            var produse = await _context.Produs.Include(p => p.Categorie).Include(p => p.Subcategorie).ToListAsync();
            ViewBag.ActionType = actionType;
            return View(produse);
        }

        [HttpGet]
        [Authorize(Policy = "ModeratorOnly")]
       
        public async Task<IActionResult> ModificaProdus(int produsId)
        {
            var produs = await _context.Produs
                .Include(p => p.Categorie)
                .Include(p => p.Subcategorie)
                .Include(p => p.ValoriAtribute)
                    .ThenInclude(v => v.AtributCategorie)
                .FirstOrDefaultAsync(p => p.ID == produsId);

            if (produs == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var categorii = await _context.Categories.Where(c => c.ParentCategorieID == null).ToListAsync();
            ViewBag.Categorii = new SelectList(categorii, "ID", "Nume");

            // incarcare subcategorii
            var subcategorii = await _context.Categories
                .Where(c => c.ParentCategorieID == produs.CategorieID)
                .ToListAsync();
            ViewBag.Subcategorii = new SelectList(subcategorii, "ID", "Nume", produs.SubcategorieID);

            // incarcare atribute dinamice
            var atributeDinamice = await _context.AtributCategorii
                .Where(ac => ac.CategorieID == produs.CategorieID || (produs.SubcategorieID.HasValue && ac.CategorieID == produs.SubcategorieID))
                .ToListAsync();

            ViewBag.AtributeDinamice = atributeDinamice;  // Poate fi folosit pentru a genera dinamic campurile de atribute in View

            return View(produs);
        }

        [HttpPost]
        [Authorize(Policy = "ModeratorOnly")]
        public async Task<IActionResult> ModificaProdus(Produs produs, IFormFile? Poza)
        {
            var existingProdus = await _context.Produs.AsNoTracking().FirstOrDefaultAsync(p => p.ID == produs.ID);

            if (ModelState.IsValid)
            {

                // verif daca s-a incarcat alta poza
                if (Poza != null && Poza.Length > 0)
                {
                    var sanitizedProductName = string.Concat(produs.Nume.Where(c => !Path.GetInvalidFileNameChars().Contains(c))).Replace(" ", "_");
                    var fileName = $"{sanitizedProductName}_{produs.ID}_{Path.GetRandomFileName()}{Path.GetExtension(Poza.FileName)}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "src", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Poza.CopyToAsync(stream);
                    }
                    produs.Poza = fileName;
                }
                // pastram poza veche
                else
                {
                    if (existingProdus != null)
                    {
                        produs.Poza = existingProdus.Poza; 
                    }
                }


                // luam produsul vechi pentru a verifica schimbarile in categorie/subcategorie
                
                bool pretModificat = existingProdus.Pret != produs.Pret;
                bool pretScazut = produs.Pret < existingProdus.Pret;
                produs.PreviousPrice = existingProdus.Pret;

                SaveDynamicAttributes(produs.ID, Request.Form);

                _context.Produs.Update(produs);
                await _context.SaveChangesAsync();

                if (pretModificat && pretScazut)
                {
                    await VerificaPreturi(produs);
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorii = new SelectList(await _context.Categories.ToListAsync(), "ID", "Nume", produs.CategorieID);
            return View(produs);
        }


        [HttpPost]
        [Authorize(Policy = "ModeratorOnly")]
        public IActionResult stergeProdus(int produsId)
        {
            Produs? produs = _context.Produs.Where(p => produsId == p.ID)
                .Include(p => p.Categorie).FirstOrDefault();

            if (produs == null)
            {
                RedirectToAction("Error", "Home");
                TempData["ErrorMessage"] = "Produsul nu a fost gasit!";
                return RedirectToAction("ListaProduse", new { actionType = "sterge" });


            }

            _context.Remove(produs);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Produsul a fost șters cu succes.";
            return RedirectToAction("ListaProduse", new { actionType = "sterge" }); // Ca sa iNTre Pe acTIuNEa undE SE aFiSEAZa taBELul IMpreuNa cu butonul de stergere
                                                                                    //altfel se afiseaza tabelul fara buton

            //  return RedirectToAction("ListaProduse");
        }

       

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var produse = await _context.Produs.Include(prod => prod.Categorie).Include(prod => prod.Subcategorie).ToListAsync();
            if (produse == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(produse);
        }

        [HttpGet]
        public async Task<JsonResult> GetSubcategorii(int categorieId)
        {
            var subcategorii = await _context.Categories
                .Where(c => c.ParentCategorieID == categorieId)
                .Select(c => new { c.ID, c.Nume })
                .ToListAsync();

            return Json(subcategorii);
        }

        [HttpGet]
        public async Task<IActionResult> GetAtributeDinamice(int categorieId, int? subcategorieId)
        {
            var categorie = await _context.Categories
                .Include(c => c.Atribute)
                .FirstOrDefaultAsync(c => c.ID == categorieId);

            if (categorie == null)
            {
                return NotFound();
            }

            var subcategorieAtribute = new List<AtributCategorie>();
            if (subcategorieId.HasValue)
            {
                var subcategorie = await _context.Categories
                    .Include(c => c.Atribute)
                    .FirstOrDefaultAsync(c => c.ID == subcategorieId.Value);

                if (subcategorie != null)
                {
                    subcategorieAtribute = subcategorie.Atribute.ToList();
                }
            }

            var model = new
            {
                CategorieAtribute = categorie.Atribute,
                SubcategorieAtribute = subcategorieAtribute
            };

            return PartialView("_AtributeDinamice", model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AdaugaRecenzie(int produsId, string textRecenzie, int evaluare)
        {
            var userId = int.Parse(User.FindFirst("IdUserLogat").Value);

            var review = new Recenzie
            {
                ProdusID = produsId,
                UtilizatorID = userId,
                TextRecenzie = textRecenzie,
                Evaluare = evaluare,
                DataRecenzie = DateTime.Now
            };

            _context.Recenzie.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detalii", new { prodID = produsId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ModificaRecenzie(int produsId, string textRecenzie, int evaluare)
        {
            var userId = int.Parse(User.FindFirst("IdUserLogat").Value);
            var review = await _context.Recenzie.FirstOrDefaultAsync(r => r.ProdusID == produsId && r.UtilizatorID == userId);

            if (review == null)
            {
                return NotFound();
            }

            review.TextRecenzie = textRecenzie;
            review.Evaluare = evaluare;
            review.DataRecenzie = DateTime.Now;

            _context.Recenzie.Update(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detalii", new { prodID = produsId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AdaugaLaFavorite(int produsId)
        {
            var userId = int.Parse(User.FindFirst("IdUserLogat").Value);

            var favorite = await _context.Favorite.FirstOrDefaultAsync(f => f.ProdusID == produsId && f.UtilizatorID == userId);
            if (favorite == null)
            {
                favorite = new Favorite
                {
                    ProdusID = produsId,
                    UtilizatorID = userId
                };

                _context.Favorite.Add(favorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Detalii", new { prodID = produsId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EliminaDinFavorite(int produsId)
        {
            var userId = int.Parse(User.FindFirst("IdUserLogat").Value);

            var favorite = await _context.Favorite.FirstOrDefaultAsync(f => f.ProdusID == produsId && f.UtilizatorID == userId);
            if (favorite != null)
            {
                _context.Favorite.Remove(favorite);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Detalii", new { prodID = produsId });
        }

        [HttpGet]
        public async Task<JsonResult> GetFavoriteStatus(int produsId)
        {
            var userId = User.Identity.IsAuthenticated ? int.Parse(User.FindFirst("IdUserLogat").Value) : (int?)null;

            if (!userId.HasValue)
            {
                return Json(new { isFavorite = false });
            }

            var isFavorite = await _context.Favorite.AnyAsync(f => f.ProdusID == produsId && f.UtilizatorID == userId);

            return Json(new { isFavorite = isFavorite });
        }

        private async Task VerificaPreturi(Produs produs)
        {
            var utilizatori = await _context.Favorite
                .Where(f => f.ProdusID == produs.ID)
                .Select(f => f.Utilizator)
                .ToListAsync();

            foreach (var utilizator in utilizatori)
            {
                if (utilizator != null)
                {
                    
                    var notificare = new Notificare
                    {
                        UtilizatorID = utilizator.ID,
                        ProdusID = produs.ID,
                        PretNou = produs.Pret,
                        PretVechi = produs.PreviousPrice,
                        ProdusNume = produs.Nume,
                        DataNotificare = DateTime.Now,
                        Citita = false
                    };

                    _context.Notificari.Add(notificare);

                   
                    TrimiteNotificareEmail(utilizator.Email, produs.Nume, produs.Pret,produs.PreviousPrice);
                }
            }

            await _context.SaveChangesAsync();
        }

        private void TrimiteNotificareEmail(string email, string numeProdus, decimal pretNou,decimal pretVechi)
        {
            var fromAddress = new MailAddress("lazarwilliam10@yahoo.com", "Notificare generata automat");
            var toAddress = new MailAddress(email);
            const string fromPassword = "kastbjzegldunhha";
            string subject = $"Alerta de pret! {numeProdus} acum la un pret redus!";
            string body = $@"
    <html>
    <head>
        <style>
            body {{ font-family: 'Arial'; }}
            .header {{ color: #FF4500; font-size: 24px; }}
            .content {{ color: #333; font-size: 16px; }}
            .footer {{ color: #777; font-size: 12px; }}
        </style>
    </head>
    <body>
        <div class='header'>Super Oferta!</div>
        <p class='content'>Draga client,</p>
        <p class='content'>Avem vesti grozave! Prețul produsului <strong>{numeProdus}</strong> a fost redus.</p>
        <p class='content'>Profita acum de noua oferta de la <strong>{pretVechi} </strong>: <strong>{pretNou} lei</strong>!</p>
        <p class='content'>Viziteaza magazinul nostru pentru a profita de aceasta oferta si pentru a explora alte reduceri.</p>
        <div class='footer'>Cu respect,<br/>Echipa ta de la Magazinul Online</div>
    </body>
    </html>";

            var smtp = new SmtpClient
            {
                Host = "smtp.mail.yahoo.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                IsBodyHtml = true, // Specificam ca mesajul e în format HTML
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

    }
}
