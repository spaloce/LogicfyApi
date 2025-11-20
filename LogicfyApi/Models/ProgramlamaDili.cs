using System.ComponentModel;

namespace LogicfyApi.Models
{
    public class ProgramlamaDili : BaseEntity
    {
        public string Ad { get; set; }
        public string Kod { get; set; }   // "javascript", "python", "csharp", "html", "css"
        public string IkonUrl { get; set; }
        public bool AktifMi { get; set; } = true;

        // Navigation Properties
        public ICollection<Unite> Uniteler { get; set; } = new List<Unite>();
    }

}
