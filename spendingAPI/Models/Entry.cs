using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spendingAPI.Models
{
    public class Entry
    {
        public int Id { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }
        public int Day { get; set; }
        public double Amount { get; set; }
        public Category Category { get; set; }
        public string Comment { get; set; }

        public IdentityUser User { get; set; }
        public string UserId { get; set; }
    }

    public enum Category
    {
        GROCERIES,
        RECREATION,
        EATING_OUT,
        PET_SUPPLIES,
        BOOKS,
        LEARNING_MATERIAL,
        HOME,
        GIFTS,
        ALCOHOL,
        RENT,
        SUBSCRIPTIONS,
        OTHER
    }
}
