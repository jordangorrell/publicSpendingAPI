using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using spendingAPI.DTOs;
using spendingAPI.Models;
using spendingAPI.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace spendingAPI.Services
{
    public class EntryService : IEntryService
    {
        private readonly IEntryRepository _entryRepository;

        public EntryService(IEntryRepository entryRepository)
        {
            _entryRepository = entryRepository;
        }

        public async Task<IEnumerable<Entry>> GetEntries(SearchOptions options, string userId)
        {
            if (ValidOptions(options)) return await _entryRepository.GetEntries(options, userId);
            else throw new ArgumentException("Search options not valid.");
        }

        public async Task<Entry> AddEntry(Entry entry, string userId)
        {
            if (ValidEntry(entry)) return await _entryRepository.AddEntry(entry, userId);
            else throw new ArgumentException("Entry object invalid.");
        }

        public async Task<Entry> UpdateEntry(int id, EntryForUpdateDto entryForUpdate, string userId)
        {
            if (ValidEntry(entryForUpdate)) return await _entryRepository.UpdateEntry(id, entryForUpdate, userId);
            else throw new ArgumentException("Entry couldn't be updated, invalid fields.");
        }

        public async Task<Entry> DeleteEntry(int id, string userId)
        {
            return await _entryRepository.DeleteEntry(id, userId);
        }

        private bool ValidOptions(SearchOptions options)
        {
            if (options.Year != null && !ValidYear(options.Year.Value)) return false;
            if (options.Month != null && !ValidMonth(options.Month.Value)) return false;
            if (options.LowerThreshold != null && !ValidAmount(options.LowerThreshold.Value)) return false;
            if (options.UpperThreshold != null && !ValidAmount(options.UpperThreshold.Value)) return false;

            return true;
        }

        private bool ValidEntry(Entry entry)
        {
            if (!ValidDate(entry.Year, entry.Month, entry.Day))
                return false;

            if (!ValidAmount(entry.Amount)) return false;

            return true;
        }

        private bool ValidEntry(EntryForUpdateDto entry)
        {
            if (!ValidDate(entry.Year.Value, entry.Month.Value, entry.Day.Value))
                return false;

            if (!ValidAmount(entry.Amount.Value)) return false;

            return true;
        }

        private bool ValidDate(int year, int month, int day)
        {
            if (!ValidMonth(month)) return false;
            if (!ValidYear(year)) return false;

            int monthLimit;
            int[] monthsWith30Days = { 1, 3, 5, 7, 8, 10, 12 };
            int[] monthsWith31Days = { 4, 6, 9, 11 };

            if (monthsWith30Days.Contains(month)) monthLimit = 30;
            else if (monthsWith31Days.Contains(month)) monthLimit = 31;
            else // February
            {
                if (year % 4 == 0) monthLimit = 29; // Leap Year
                else monthLimit = 28;
            }

            return day >= 1 && day <= monthLimit;
        }

        private bool ValidAmount(double amount)
        {
            return amount >= 0;
        }

        private bool ValidMonth(int month) { return month >= 1 && month <= 12; }

        private bool ValidYear(int year) { return year <= DateTime.Now.Year && year > 1900; }

    }
}
