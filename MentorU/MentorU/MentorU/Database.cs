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
            _context.CreateTableAsync<Users>().Wait();
        }

        public Task<List<Users>> GetUserAsync()
        {
            return _context.Table<Users>().ToListAsync();
        }

        public Task<int> SaveUserAsync(Users user)
        {
            return _context.InsertAsync(user);
        }
    }
}
