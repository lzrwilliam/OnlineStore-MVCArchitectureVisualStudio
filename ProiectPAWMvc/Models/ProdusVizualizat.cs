using ProiectPAW.wwwroot.Models;

namespace ProiectPAW.Models;

public class ProdusVizualizat
{
    public int ID { get; set; }
    public int UtilizatorID { get; set; }
    public int ProdusID { get; set; }
    public DateTime DataVizualizare { get; set; }
    public Utilizator? Utilizator { get; set; }
    public Produs? Produs { get; set; }
}
