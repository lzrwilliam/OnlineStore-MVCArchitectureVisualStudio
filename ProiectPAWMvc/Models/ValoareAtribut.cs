using ProiectPAW.wwwroot.Models;

namespace ProiectPAWMvc.Models
{
    public class ValoareAtribut
    {
        public int ID { get; set; }
        public int ProdusID { get; set; }
        public Produs Produs { get; set; }
        public int AtributCategorieID { get; set; }
        public AtributCategorie AtributCategorie { get; set; }
        public string Valoare { get; set; }
    }
}
