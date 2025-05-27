using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces {
    public interface ICandidateRepository {
        Task<List<Candidate>> GetAllCandidatesWithVotesAsync();
        Task<Candidate> GetCandidateWithVotesAsync(int id);
        Task<Candidate> CreateCandidateAsync(Candidate candidate);
        Task<Vote> AddVoteAsync(Vote vote);
        Task<Candidate> GetCandidateByIdAsync(int id);

        Task<List<Vote>> GetAllVotesAsync();
        Task<Vote> GetVoteByIdAsync(int id);
        Task<List<Vote>> GetVotesByCandidateIdAsync(int candidateId);

        Task<Candidate> UpdateCandidateAsync(Candidate candidate);
        Task<bool> CheckForDuplicateCandidateAsync(int excludeId, string name, string party, string position, string electionYear);
    }
}
