using ProiectPAW.Models;
using ProiectPAW.wwwroot.Models;

namespace ProiectPAWMvc.Models;
public class Notificare
{
    public int ID { get; set; }
    public int UtilizatorID { get; set; }
    public int ProdusID { get; set; }
    public string ProdusNume { get; set; }
    public decimal PretNou { get; set; }
    public decimal PretVechi { get; set; }
    public DateTime DataNotificare { get; set; }
    public bool Citita { get; set; } 
    public Produs Produs { get; set; }
    public Utilizator Utilizator { get; set; }
}