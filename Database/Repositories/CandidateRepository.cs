using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories {
    public class CandidateRepository : ICandidateRepository {
        private readonly AppDbContext _context;

        public CandidateRepository(AppDbContext context) {
            _context = context;
        }

        public async Task<List<Candidate>> GetAllCandidatesWithVotesAsync() {
            return await _context.Candidates
                .Include(c => c.Votes)
                .ToListAsync();
        }

        public async Task<Candidate> GetCandidateWithVotesAsync(int id) {
            return await _context.Candidates
                .Include(c => c.Votes)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Candidate> CreateCandidateAsync(Candidate candidate) {
            await _context.Candidates.AddAsync(candidate);
            await _context.SaveChangesAsync();
            return candidate;
        }
        public async Task<Vote> AddVoteAsync(Vote vote) {
            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync();
            return vote;
        }

        public async Task<Candidate> GetCandidateByIdAsync(int id) {
            return await _context.Candidates.FindAsync(id);
        }

        public async Task<List<Vote>> GetAllVotesAsync() {
            return await _context.Votes
                .Include(v => v.Candidate)
                .ToListAsync();
        }

        public async Task<Vote> GetVoteByIdAsync(int id) {
            return await _context.Votes
                .Include(v => v.Candidate)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<List<Vote>> GetVotesByCandidateIdAsync(int candidateId) {
            return await _context.Votes
                .Where(v => v.CandidateId == candidateId)
                .ToListAsync();
        }

        public async Task<Candidate> UpdateCandidateAsync(Candidate candidate)
        {
            _context.Candidates.Update(candidate);
            await _context.SaveChangesAsync();
            return await GetCandidateWithVotesAsync(candidate.Id);
        }

        public async Task<bool> CheckForDuplicateCandidateAsync(int excludeId, string name, string party, string position, string electionYear)
        {
            return await _context.Candidates
                .AnyAsync(c => c.Id != excludeId &&
                              c.Name.ToLower() == name.ToLower() &&
                              c.Party.ToLower() == party.ToLower() &&
                              c.Position.ToLower() == position.ToLower() &&
                              c.ElectionYear == electionYear);
        }
    }
}
