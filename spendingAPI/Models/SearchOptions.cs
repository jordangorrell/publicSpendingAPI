using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spendingAPI.Models
{
    public class SearchOptions
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public Category? Category { get; set; }
        public double? LowerThreshold { get; set; }
        public double? UpperThreshold { get; set; }
        public bool ExcludeRentAndSubscriptions { get; set; }
    }
}
