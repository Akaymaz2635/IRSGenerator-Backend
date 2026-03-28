namespace IRSGenerator.Shared.Dtos.User;

public class UserUpdateDto
{
    public string? Name { get; set; }
    public string? Role { get; set; }
    public string? Password { get; set; }
    public bool? Active { get; set; }
}
