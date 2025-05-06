using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs {
    public class CandidateDto {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Party { get; set; }
        public string Position { get; set; }
        public string ElectionYear { get; set; }
        public string Biography { get; set; }
        public int TotalVotes { get; set; }
        public List<VoteDto> Votes { get; set; } = new List<VoteDto>();
    }

    public class VoteDto {
        public int Id { get; set; }
        public string PollingStation { get; set; }
        public int Count { get; set; }
        public DateTime RecordedAt { get; set; }
    }

    public class CreateCandidateDto {
        public string Name { get; set; }
        public string Party { get; set; }
        public string Position { get; set; }
        public string ElectionYear { get; set; }
        public string Biography { get; set; }
    }

    public class CreateVoteDto {
        public int CandidateId { get; set; }
        public string PollingStation { get; set; }
        public int Count { get; set; }
    }
}
