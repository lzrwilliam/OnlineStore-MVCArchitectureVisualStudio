using ProiectPAWMvc.Models;

namespace ProiectPAW.wwwroot.Models;



public class Categorie
{
    public int ID { get; set; }
    public string Nume { get; set; }
    public int? ParentCategorieID { get; set; }
    public Categorie? ParentCategorie { get; set; }
    public ICollection<Categorie> Subcategorii { get; set; } = new List<Categorie>();
    public ICollection<Produs> Produse { get; set; } = new List<Produs>();

    public ICollection<AtributCategorie> Atribute { get; set; } = new List<AtributCategorie>();


}
