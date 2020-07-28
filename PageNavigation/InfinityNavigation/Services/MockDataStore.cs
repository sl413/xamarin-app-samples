using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfinityNavigation.Models;

namespace InfinityNavigation.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        private static List<Item> Items => GenerateNewItems();

        private static List<Item> GenerateNewItems()
        {
            var items = new List<Item>();
            for (var i = 0; i < 33; i++)
            {
                var guid = Guid.NewGuid().ToString();
                items.Add(new Item {Id = guid, Text = guid, Description = "description"});
            }

            return items;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(Items);
        }
    }
}