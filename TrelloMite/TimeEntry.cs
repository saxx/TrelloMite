using System;

namespace TrelloMite
{
    public class TimeEntry
    {
        public string Project { get; set; }
        public string Customer { get; set; }
        public string Service { get; set; }

        public DateTime Date { get; set; }
        public int Minutes { get; set; }
        public string Notes { get; set; }
    }
}
