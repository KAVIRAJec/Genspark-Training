using FAQChatBot.Models.DTOs;
using FAQChatBot.Models;
using FAQChatBot.Interfaces;
using FAQChatBot.Contexts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FAQChatBot.Repositories
{
    public class UserRepository : Repository<int, User>
    {
        public UserRepository(FAQContext context) : base(context)
        {
        }
        
        public override async Task<User> Get(int id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
            return user ?? throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            if (users == null || users.Count() == 0)
                throw new KeyNotFoundException("No users found in the database.");
            return users;
        }
    }
}