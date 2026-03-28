using System.Collections.ObjectModel;

namespace IRSGenerator.Core.Entities
{
    public class VisualProject : BaseEntity
    {
        public string Name { get; set; } = "";
        public bool Active { get; set; } = true;

        public ICollection<Inspection> Inspections { get; set; } = new Collection<Inspection>();
    }
}
