namespace TeamTaskManagerApi.DTOs;

public class RegisterUserDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class LoginUserDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class UserResponseDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
}

public class LoginResponseDto
{
    public required UserResponseDto User { get; set; }
    public required string Token { get; set; }
}
