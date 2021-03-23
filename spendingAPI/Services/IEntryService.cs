using spendingAPI.DTOs;
using spendingAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace spendingAPI.Services
{
    public interface IEntryService
    {
        Task<Entry> AddEntry(Entry entry, string userId);
        Task<Entry> DeleteEntry(int id, string userId);
        Task<IEnumerable<Entry>> GetEntries(SearchOptions options, string userId);
        Task<Entry> UpdateEntry(int id, EntryForUpdateDto entryForUpdate, string userId);
    }
}