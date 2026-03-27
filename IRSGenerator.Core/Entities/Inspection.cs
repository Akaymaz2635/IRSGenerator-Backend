using System.Collections.ObjectModel;

namespace IRSGenerator.Core.Entities
{
    public class Inspection : BaseEntity
    {
        public long IrsProjectId { get; set; }
        public long? InspectorId { get; set; }
        public string Status { get; set; } = "open";
        public string? Notes { get; set; }

        // Navigation
        public IRSProject IrsProject { get; set; } = null!;
        public User? Inspector { get; set; }
        public ICollection<Defect> Defects { get; set; } = new Collection<Defect>();
        public ICollection<Photo> Photos { get; set; } = new Collection<Photo>();
    }
}
