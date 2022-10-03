using DatingApp.API.Enities;

namespace DatingApp.API.Service
{
    public interface ITokenService{
        string CreateToken(string username);
    }
}