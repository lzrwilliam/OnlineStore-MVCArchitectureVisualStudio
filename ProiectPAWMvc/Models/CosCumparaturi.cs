using ProiectPAW.wwwroot.Models;
using System.ComponentModel.DataAnnotations;

namespace ProiectPAW.Models;

public class CosCumparaturi
{
    public int ID { get; set; }
    public int UtilizatorID { get; set; }
    public int ProdusID { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Cantitatea trebuie sa fie cel putin 1!")]
    public int Cantitate { get; set; }
    public Utilizator Utilizator { get; set; }
    public Produs? Produs { get; set; }
}
