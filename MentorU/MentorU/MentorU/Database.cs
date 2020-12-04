using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using MentorU.Models;

namespace MentorU
{
    public class Database
    {
        readonly SQLiteAsyncConnection _context;

        public Database(string dbPath)
        {
            _context = new SQLiteAsyncConnection(dbPath);
            _context.CreateTableAsync<Profile>().Wait();
        }

        public Task<List<Profile>> GetUserAsync()
        {
            return _context.Table<Profile>().ToListAsync();
        }

        public Task<int> SaveUserAsync(Profile user)
        {
            return _context.InsertAsync(user);
        }
    }
}
