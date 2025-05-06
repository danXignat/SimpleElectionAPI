using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Core.Services {

    public class CandidateService {
        private readonly ICandidateRepository _candidateRepository;

        public CandidateService(ICandidateRepository candidateRepository) {
            _candidateRepository = candidateRepository;
        }

        public async Task<List<CandidateDto>> GetAllCandidatesWithVotesAsync() {
            var candidates = await _candidateRepository.GetAllCandidatesWithVotesAsync();
            return MapCandidatesToDtos(candidates);
        }

        public async Task<CandidateDto> GetCandidateWithVotesAsync(int id) {
            var candidate = await _candidateRepository.GetCandidateWithVotesAsync(id);
            if (candidate == null)
                return null;

            return MapCandidateToDto(candidate);
        }

        private List<CandidateDto> MapCandidatesToDtos(List<Candidate> candidates) {
            var candidateDtos = new List<CandidateDto>();

            foreach (var candidate in candidates) {
                candidateDtos.Add(MapCandidateToDto(candidate));
            }

            return candidateDtos;
        }

        private CandidateDto MapCandidateToDto(Candidate candidate) {
            var candidateDto = new CandidateDto {
                Id = candidate.Id,
                Name = candidate.Name,
                Party = candidate.Party,
                Position = candidate.Position,
                ElectionYear = candidate.ElectionYear,
                Biography = candidate.Biography,
                TotalVotes = candidate.Votes.Sum(v => v.Count),
                Votes = candidate.Votes.Select(v => new VoteDto {
                    Id = v.Id,
                    PollingStation = v.PollingStation,
                    Count = v.Count,
                    RecordedAt = v.RecordedAt
                }).ToList()
            };

            return candidateDto;
        }

        public async Task<CandidateDto> CreateCandidateAsync(CreateCandidateDto createCandidateDto) {
            var candidate = new Candidate {
                Name = createCandidateDto.Name,
                Party = createCandidateDto.Party,
                Position = createCandidateDto.Position,
                ElectionYear = createCandidateDto.ElectionYear,
                Biography = createCandidateDto.Biography
            };

            var createdCandidate = await _candidateRepository.CreateCandidateAsync(candidate);

            return new CandidateDto {
                Id = createdCandidate.Id,
                Name = createdCandidate.Name,
                Party = createdCandidate.Party,
                Position = createdCandidate.Position,
                ElectionYear = createdCandidate.ElectionYear,
                Biography = createdCandidate.Biography,
                TotalVotes = 0,
                Votes = new List<VoteDto>()
            };
        }
    }
}
