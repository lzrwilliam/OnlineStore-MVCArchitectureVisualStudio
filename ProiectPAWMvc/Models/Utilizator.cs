using ProiectPAWMvc.Logic;

namespace ProiectPAW.Models
{
    public class Utilizator
    {
        public int ID { get; set; }
        public string Nume { get; set; }
        public string Email { get; set; }
        public string Parola { get; set; }
        public TipUser TipUtilizator { get; set; } // 'vizitator', 'inregistrat', 'moderator'
    }
}

