using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectPAW.ContextModels;
using ProiectPAW.wwwroot.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProiectPAWMvc.Components
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ProdusContext _context;

        public CategoryMenuViewComponent(ProdusContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categorii = await _context.Categories.ToListAsync();
            return View(categorii);
        }
    }
}
