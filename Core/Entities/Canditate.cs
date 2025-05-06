using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities {
    public class Candidate {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Party { get; set; }
        public string Position { get; set; }
        public string ElectionYear { get; set; }
        public string Biography { get; set; }

        // Navigation property for one-to-many relationship
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }

    public class Vote {
        public int Id { get; set; }
        public string PollingStation { get; set; }
        public int Count { get; set; }
        public DateTime RecordedAt { get; set; }

        // Foreign key for one-to-many relationship
        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; }
    }
}
