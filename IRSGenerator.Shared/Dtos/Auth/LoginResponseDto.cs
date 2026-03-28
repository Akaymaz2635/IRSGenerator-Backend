namespace IRSGenerator.Shared.Dtos.Auth;

public class LoginResponseDto
{
    public bool IsAdmin { get; set; }          // snake: is_admin
    public LoginUserDto? User { get; set; }
}

public class LoginUserDto
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
}
