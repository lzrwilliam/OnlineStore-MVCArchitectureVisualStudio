using ProiectPAW.Models;
using ProiectPAWMvc.Models;
using System.ComponentModel.DataAnnotations;

namespace ProiectPAW.wwwroot.Models
{
    public class Produs
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Numele este cerut")]
        public string Nume { get; set; }
        public string Descriere { get; set; }
        public decimal Pret { get; set; }
        public decimal PreviousPrice { get; set; } 
        public int Stoc { get; set; }
        public string Brand { get; set; }
        public string? Poza { get; set; }
        public int CategorieID { get; set; }
        public int? SubcategorieID { get; set; }
        public Categorie? Categorie { get; set; }
        public Categorie? Subcategorie { get; set; }
        public ICollection<Recenzie> Recenzii { get; set; } = new List<Recenzie>();
        public ICollection<DetaliiComanda> Comenzi { get; set; } = new List<DetaliiComanda>();
        public ICollection<Favorite> Favorite { get; set; } = new List<Favorite>(); // evidenta favoritelor
        public ICollection<ValoareAtribut> ValoriAtribute { get; set; } = new List<ValoareAtribut>(); 

    }

}
