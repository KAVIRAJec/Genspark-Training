using FAQChatBot.Models.DTOs;
using FAQChatBot.Models;
using FAQChatBot.Interfaces;
using FAQChatBot.Contexts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FAQChatBot.Repositories
{
    public class FAQRepository : Repository<int, FAQ>
    {
        public FAQRepository(FAQContext context) : base(context)
        {
        }
        public override async Task<FAQ> Get(int id)
        {
            var faq = await _context.FAQs.SingleOrDefaultAsync(f => f.Id == id);
            return faq ?? throw new KeyNotFoundException($"FAQ with ID {id} not found.");
        }
        public override async Task<IEnumerable<FAQ>> GetAll()
        {
            var faqs = await _context.FAQs.ToListAsync();
            if (faqs == null || faqs.Count() == 0)
                throw new KeyNotFoundException("No FAQs in the database.");
            return faqs;
        }
    }
}