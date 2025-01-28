using ProiectPAW.wwwroot.Models;
using System.Collections.Generic;
using X.PagedList;

namespace ProiectPAWMvc.Models
{
    public class CategorieDetaliiViewModel
    {
        public Categorie Categorie { get; set; } // tine detaliile categoriei

        public int CategorieID { get; set; }
        public string CategorieNume { get; set; }
        public int? ParentCategorieID { get; set; }
        public string ParentCategorieNume { get; set; }
        public IPagedList<Produs> Produse { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Brand { get; set; }
        public Dictionary<int, string> Atribute { get; set; } //tine  atributele specifice filtrarii
        public List<int> FavoriteIds { get; set; } // tine  evidenta produselor favorite
        public List<Categorie> Subcategorii { get; set; } // tine subcategoriile
        public List<AtributDinamicFiltru> AtributeDinamice { get; set; } = new List<AtributDinamicFiltru>();

        public CategorieDetaliiViewModel()
        {
            AtributeDinamice = new List<AtributDinamicFiltru>();

            FavoriteIds = new List<int>();
            Atribute = new Dictionary<int, string>();
            Subcategorii = new List<Categorie>(); // pentru a evita problemele null

        }
    }
}
