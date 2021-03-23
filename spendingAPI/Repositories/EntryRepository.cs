using Microsoft.EntityFrameworkCore;
using spendingAPI.Data;
using spendingAPI.DTOs;
using spendingAPI.Exceptions;
using spendingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spendingAPI.Repositories
{
    public class EntryRepository : IEntryRepository
    {
        private readonly SpendingContext _context;

        public EntryRepository(SpendingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Entry>> GetEntries(SearchOptions options, string userId)
        {
            var query = _context.Entries.AsQueryable();
            query = query.Where(e => e.UserId == userId);
            
            if (options.Year != null)
                query = query.Where(e => e.Year == options.Year);

            if (options.Month != null)
                query = query.Where(e => e.Month == options.Month);

            if (options.Category != null)
                query = query.Where(e => e.Category == options.Category);

            if (options.LowerThreshold != null)
                query = query.Where(e => e.Amount >= options.LowerThreshold);

            if (options.UpperThreshold != null)
                query = query.Where(e => e.Amount <= options.UpperThreshold);

            if (options.ExcludeRentAndSubscriptions)
                query = query.Where(e => e.Category != Category.RENT && e.Category != Category.SUBSCRIPTIONS);

            query = query.OrderBy(e => e.Year)
                .ThenBy(e => e.Month)
                .ThenBy(e => e.Day)
                .ThenBy(e => e.Amount)
                .ThenBy(e => e.Category);
            // This way the order can come back predictably

            var entries = await query.ToListAsync();
            return entries;
        }

        public async Task<Entry> GetEntry(int id, string userId)
        {
            var entry = await _context.Entries.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            return entry;
        }

        public async Task<Entry> AddEntry(Entry entry, string userId)
        {
            entry.UserId = userId;
            await _context.Entries.AddAsync(entry);
            await SaveChanges();
            return entry;
        }

        public async Task<Entry> UpdateEntry(int id, EntryForUpdateDto entryForUpdate, string userId)
        {
            var entry = await GetEntry(id, userId);

            if (entry == null)
                throw new NotFoundException($"Could not find entry with Id '{id}'");

            MapEntryDtoToEntry(entry, entryForUpdate);

            await SaveChanges();

            return entry;
        }

        public async Task<Entry> DeleteEntry(int id, string userId)
        {
            var entryToDelete = await GetEntry(id, userId);

            if (entryToDelete == null)
                throw new NotFoundException($"Could not find entry with Id '{id}'");

            _context.Entries.Remove(entryToDelete);

            await SaveChanges();

            return entryToDelete;
        }

        private void MapEntryDtoToEntry(Entry entry, EntryForUpdateDto entryForUpdate)
        {
            if (entryForUpdate.Year != null) entry.Year = entryForUpdate.Year.Value;
            if (entryForUpdate.Month != null) entry.Month = entryForUpdate.Month.Value;
            if (entryForUpdate.Day != null) entry.Day = entryForUpdate.Day.Value;
            if (entryForUpdate.Amount != null) entry.Amount = entryForUpdate.Amount.Value;
            if (entryForUpdate.Category != null) entry.Category = entryForUpdate.Category.Value;
            if (entryForUpdate.Comment != null) entry.Comment = entryForUpdate.Comment;
        }

        private async Task<bool> SaveChanges()
        {
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

    }
}
