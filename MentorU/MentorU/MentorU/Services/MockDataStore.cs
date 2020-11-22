﻿using MentorU.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MentorU.Services
{
    public class MockDataStore : IDataStore<MarketplaceItem>
    {
        readonly List<MarketplaceItem> items;

        public MockDataStore()
        {
            items = new List<MarketplaceItem>()
            {
                new MarketplaceItem { Id = Guid.NewGuid().ToString(), Text = "First item", ItemPrice = 10.0, Description="This is an item description." },
                new MarketplaceItem { Id = Guid.NewGuid().ToString(), Text = "Second item", ItemPrice = 100.0,Description="This is an item description." }
            };
        }

        public async Task<bool> AddItemAsync(MarketplaceItem item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(MarketplaceItem item)
        {
            var oldItem = items.Where((MarketplaceItem arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((MarketplaceItem arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<MarketplaceItem> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<MarketplaceItem>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}