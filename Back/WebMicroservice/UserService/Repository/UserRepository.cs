using Microsoft.EntityFrameworkCore;
using UserService.Database;
using UserService.Models;

namespace UserService.Repository
{
    public class UserRepository : IUserRepository
    {
        UserDBContext _dbContext;

        public UserRepository(UserDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ChangePassword(long id, string password)
        {
            Models.User user = await _dbContext.Users.FindAsync(id);
            user.Password = password;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }

        public async Task<Models.User> Create(Models.User entity)
        {
            await _dbContext.Users.AddAsync(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public Task Delete(long id)
        {
            _dbContext.Users.Remove(_dbContext.Users.Find(id));
            return _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Models.User>> GetAll()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<Models.User> GetById(long id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<byte[]> GetUserImage(long id)
        {
            Models.User user = await _dbContext.Users.FindAsync(id);
            return user.ProfilePictureUrl;
        }

        public Task Update(Models.User entity)
        {
            Models.User newUser = _dbContext.Users.Find(entity.Id);
            newUser = entity;
            _dbContext.Users.Update(newUser);
            return _dbContext.SaveChangesAsync();
        }
    }
}
