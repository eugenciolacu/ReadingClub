using ReadingClub.Domain;

namespace ReadingClub.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Create(User user);
        Task Delete(string userEmail);
        Task<User> Get(string email);
        Task<int> CheckIfExists(string userName, string email);
        Task<User> Update(User user, string oldEmail, bool isEditPassword);
        Task<int> GetUserIdByEmail(string email);
    }
}