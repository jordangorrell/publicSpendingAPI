using spendingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spendingAPI.DTOs
{
    public class EntryForUpdateDto
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? Day { get; set; }
        public double? Amount { get; set; }
        public Category? Category { get; set; }
        public string Comment { get; set; }
    }
}
