namespace IRSGenerator.Shared.Dtos.User;

public class UserReadDto
{
    public long Id { get; set; }
    public string EmployeeId { get; set; } = "";   // snake: employee_id
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public bool Active { get; set; }
    public DateTime? CreatedAt { get; set; }
}
