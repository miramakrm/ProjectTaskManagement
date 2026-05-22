namespace ProjectTaskManagement.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(
        string userId,
        string email,
        string userName);
}