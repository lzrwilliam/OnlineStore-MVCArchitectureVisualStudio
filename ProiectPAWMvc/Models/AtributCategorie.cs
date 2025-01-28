using ProiectPAWMvc.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectPAW.wwwroot.Models

{
    public class AtributCategorie
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Numele atributului este obligatoriu")]
        public string Nume { get; set; }

        [Required(ErrorMessage = "Tipul atributului este obligatoriu")]
        public string Tip { get; set; }

        [Required]
        public int CategorieID { get; set; }

        [ForeignKey("CategorieID")]
        [NotMapped] 
        public virtual Categorie Categorie { get; set; }


        public ICollection<ValoareAtribut> ValoriAtribute { get; set; } = new List<ValoareAtribut>();
    }
}
