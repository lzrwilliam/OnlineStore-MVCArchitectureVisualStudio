using ProiectPAW.wwwroot.Models;

namespace ProiectPAW.Models;

public class Recenzie
{
    public int ID { get; set; }
    public int UtilizatorID { get; set; }
    public int ProdusID { get; set; }
    public string TextRecenzie { get; set; }
    public int Evaluare { get; set; } // intre  1 si 5
    public DateTime DataRecenzie { get; set; }
    public Utilizator? Utilizator { get; set; }
    public Produs? Produs { get; set; }
}
