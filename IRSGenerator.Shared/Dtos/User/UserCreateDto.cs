namespace IRSGenerator.Shared.Dtos.User;

public class UserCreateDto
{
    public string EmployeeId { get; set; } = "";   // snake: employee_id
    public string Name { get; set; } = "";
    public string Role { get; set; } = "inspector";
    public string Password { get; set; } = "";
}
