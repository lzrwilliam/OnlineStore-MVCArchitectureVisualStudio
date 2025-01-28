namespace ProiectPAWMvc.Models
{
    public class AtributDinamicFiltru
    {
        public int AtributCategorieID { get; set; }
        public string Nume { get; set; }
        public string Tip { get; set; }
        public string? ValoareMinima { get; set; }
        public string? ValoareMaxima { get; set; }
        public string? ValoareCurenta { get; set; }
        public bool? ValoareBoolean { get; set; }
    }
}
