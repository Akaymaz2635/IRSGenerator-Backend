using System.Collections.ObjectModel;

namespace IRSGenerator.Core.Entities
{
    public class Inspection : BaseEntity
    {
        // ── WPF entegrasyonu için orijinal FK (opsiyonel) ─────────────────
        public long? IrsProjectId { get; set; }

        // ── QualiSight UI için proje bağlantısı ───────────────────────────
        public long? VisualProjectId { get; set; }

        // ── Muayene'ye özgü metin alanları ───────────────────────────────
        public string? PartNumber { get; set; }
        public string? SerialNumber { get; set; }
        public string? OperationNumber { get; set; }
        public string? Inspector { get; set; }

        public string Status { get; set; } = "open";
        public string? Notes { get; set; }

        // Navigation
        public IRSProject? IrsProject { get; set; }
        public VisualProject? VisualProject { get; set; }
        public User? InspectorUser { get; set; }
        public long? InspectorId { get; set; }

        public ICollection<Defect> Defects { get; set; } = new Collection<Defect>();
        public ICollection<Photo> Photos { get; set; } = new Collection<Photo>();
    }
}
