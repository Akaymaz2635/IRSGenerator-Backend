using System.Collections.ObjectModel;

namespace IRSGenerator.Core.Entities
{
    public class IRSProject : BaseEntity
    {
        public string ProjectType { get; set; } = default!;
        public string PartNumber { get; set; } = default!;
        public int Operation { get; set; }
        public string SerialNumber { get; set; } = default!;
        public string OpSheetPath { get; set; } = default!;

        // Owner
        public long OwnerId { get; set; }
        public User? Owner { get; set; }

        // Children
        public ICollection<Character> Characters { get; set; }
        public ICollection<Inspection> Inspections { get; set; } = new Collection<Inspection>();

        public IRSProject()
        {
            Characters = new Collection<Character>();
        }
    }
}
