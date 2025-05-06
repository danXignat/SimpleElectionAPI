using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services {
    public class VoteService {
        private readonly ICandidateRepository _candidateRepository;

        public VoteService(ICandidateRepository candidateRepository) {
            _candidateRepository = candidateRepository;
        }

        public async Task<VoteDto> AddVoteAsync(CreateVoteDto createVoteDto) {
            var candidate = await _candidateRepository.GetCandidateByIdAsync(createVoteDto.CandidateId);
            if (candidate == null) {
                throw new ArgumentException($"Candidate with ID {createVoteDto.CandidateId} not found");
            }

            var vote = new Vote {
                CandidateId = createVoteDto.CandidateId,
                PollingStation = createVoteDto.PollingStation,
                Count = createVoteDto.Count,
                RecordedAt = DateTime.UtcNow
            };

            var savedVote = await _candidateRepository.AddVoteAsync(vote);

            return new VoteDto {
                Id = savedVote.Id,
                PollingStation = savedVote.PollingStation,
                Count = savedVote.Count,
                RecordedAt = savedVote.RecordedAt
            };
        }

        public async Task<List<VoteDto>> GetAllVotesAsync() {
            var votes = await _candidateRepository.GetAllVotesAsync();
            return votes.Select(v => new VoteDto {
                Id = v.Id,
                PollingStation = v.PollingStation,
                Count = v.Count,
                RecordedAt = v.RecordedAt
            }).ToList();
        }

        public async Task<VoteDto> GetVoteByIdAsync(int id) {
            var vote = await _candidateRepository.GetVoteByIdAsync(id);
            if (vote == null)
                return null;

            return new VoteDto {
                Id = vote.Id,
                PollingStation = vote.PollingStation,
                Count = vote.Count,
                RecordedAt = vote.RecordedAt
            };
        }

        public async Task<List<VoteDto>> GetVotesByCandidateIdAsync(int candidateId) {
            var votes = await _candidateRepository.GetVotesByCandidateIdAsync(candidateId);
            return votes.Select(v => new VoteDto {
                Id = v.Id,
                PollingStation = v.PollingStation,
                Count = v.Count,
                RecordedAt = v.RecordedAt
            }).ToList();
        }

        public async Task<bool> CandidateExistsAsync(int candidateId) {
            var candidate = await _candidateRepository.GetCandidateByIdAsync(candidateId);
            return candidate != null;
        }
    }
}
