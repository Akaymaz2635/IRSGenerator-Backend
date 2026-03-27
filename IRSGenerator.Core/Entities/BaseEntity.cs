namespace IRSGenerator.Core.Entities;

public abstract class BaseEntity
{
    public long Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? CreatedById { get; set; }
    public User? CreatedByUser { get; set; }
    public long? UpdatedById { get; set; }
    public User? UpdatedByUser { get; set; }
}
