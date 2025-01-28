using ProiectPAW.wwwroot.Models;

namespace ProiectPAWMvc.Models
{
    public class DetaliiComanda
    {
        public int ID { get; set; }

        public int ComandaID { get; set; }

        public int? ProdusID { get; set; }

        public int Cantitate { get; set; }

        public decimal PretUnitate { get; set; }
        public string NumeProdus { get; set; }  

        public Comanda Comanda { get; set; }

        public Produs Produs { get; set; }
    }
}
