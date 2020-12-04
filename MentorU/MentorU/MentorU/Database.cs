using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using MentorU.Models;

namespace MentorU
{
    class Database
    {
        readonly SQLiteAsyncConnection _context;

        public Database(string dbPath)
        {
            _context = new SQLiteAsyncConnection(dbPath);
            _context.CreateTableAsync<User>().Wait();
        }

        public Task<List<User>> GetUserAsync()
        {
            return _context.Table<User>().ToListAsync();
        }

        public Task<int> SaveUserAsync(User user)
        {
            return _context.InsertAsync(user);
        }
    }
}
