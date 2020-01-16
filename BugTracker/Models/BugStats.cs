using System;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class BugStats
    {
        [DataType(DataType.Date)]
        public DateTime? FileDate { get; set; }

        public int FileCount { get; set; }
    }
}