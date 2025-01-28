using ProiectPAW.Models;

namespace ProiectPAWMvc.Models
{
    public class Comanda
    {
        public int ID { get; set; }

        public int UtilizatorID { get; set; }

        public DateTime DataComanda { get; set; }

        public decimal TotalComanda { get; set; }

        public Utilizator Utilizator { get; set; }

        public List<DetaliiComanda> DetaliiComenzi { get; set; } = new List<DetaliiComanda>();
    }
}
