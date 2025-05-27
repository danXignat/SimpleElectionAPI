using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;

namespace Core.Services
{
    public class CandidateService
    {
        private readonly ICandidateRepository _candidateRepository;

        public CandidateService(ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }

        public async Task<CandidateDto?> UpdateCandidateAsync(int id, UpdateCandidateDto updateCandidateDto)
        {
            var existingCandidate = await _candidateRepository.GetCandidateByIdAsync(id);
            if (existingCandidate == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(updateCandidateDto.ElectionYear))
            {
                if (!int.TryParse(updateCandidateDto.ElectionYear, out var year) || year < 1900 || year > DateTime.Now.Year + 10)
                {
                    throw new ArgumentException("Invalid election year provided.");
                }
            }

            if (!string.IsNullOrWhiteSpace(updateCandidateDto.Name) ||
                !string.IsNullOrWhiteSpace(updateCandidateDto.Party) ||
                !string.IsNullOrWhiteSpace(updateCandidateDto.Position) ||
                !string.IsNullOrWhiteSpace(updateCandidateDto.ElectionYear))
            {
                var duplicateCheck = await _candidateRepository.CheckForDuplicateCandidateAsync(
                    id,
                    updateCandidateDto.Name ?? existingCandidate.Name,
                    updateCandidateDto.Party ?? existingCandidate.Party,
                    updateCandidateDto.Position ?? existingCandidate.Position,
                    updateCandidateDto.ElectionYear ?? existingCandidate.ElectionYear
                );

                if (duplicateCheck)
                {
                    throw new InvalidOperationException("A candidate with similar details already exists for this election.");
                }
            }

            if (!string.IsNullOrWhiteSpace(updateCandidateDto.Name))
                existingCandidate.Name = updateCandidateDto.Name.Trim();

            if (!string.IsNullOrWhiteSpace(updateCandidateDto.Party))
                existingCandidate.Party = updateCandidateDto.Party.Trim();

            if (!string.IsNullOrWhiteSpace(updateCandidateDto.Position))
                existingCandidate.Position = updateCandidateDto.Position.Trim();

            if (!string.IsNullOrWhiteSpace(updateCandidateDto.ElectionYear))
                existingCandidate.ElectionYear = updateCandidateDto.ElectionYear.Trim();

            if (updateCandidateDto.Biography != null)
                existingCandidate.Biography = updateCandidateDto.Biography.Trim();

            var updatedCandidate = await _candidateRepository.UpdateCandidateAsync(existingCandidate);
            return MapCandidateToDto(updatedCandidate);
        }

        public async Task<PagedResult<CandidateDto>> GetFilteredCandidatesAsync(
            CandidateFilterCriteria filterCriteria,
            PaginationCriteria paginationCriteria,
            SortCriteria sortCriteria)
        {
            var allCandidates = await _candidateRepository.GetAllCandidatesWithVotesAsync();
            var candidateDtos = MapCandidatesToDtos(allCandidates);

            var filteredCandidates = ApplyFilters(candidateDtos, filterCriteria);

            var sortedCandidates = ApplySorting(filteredCandidates, sortCriteria);

            var totalCount = sortedCandidates.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / paginationCriteria.PageSize);

            var pagedCandidates = sortedCandidates
                .Skip((paginationCriteria.Page - 1) * paginationCriteria.PageSize)
                .Take(paginationCriteria.PageSize)
                .ToList();

            return new PagedResult<CandidateDto>
            {
                Data = pagedCandidates,
                TotalCount = totalCount,
                Page = paginationCriteria.Page,
                PageSize = paginationCriteria.PageSize,
                TotalPages = totalPages,
                HasNextPage = paginationCriteria.Page < totalPages,
                HasPreviousPage = paginationCriteria.Page > 1
            };
        }

        private IEnumerable<CandidateDto> ApplyFilters(List<CandidateDto> candidates, CandidateFilterCriteria filterCriteria)
        {
            var filtered = candidates.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filterCriteria.Party))
            {
                filtered = filtered.Where(c => c.Party != null &&
                    c.Party.Contains(filterCriteria.Party, StringComparison.OrdinalIgnoreCase));
            }

            if (filterCriteria.ElectionYear.HasValue)
            {
                filtered = filtered.Where(c => c.ElectionYear != null &&
                    c.ElectionYear == filterCriteria.ElectionYear.Value.ToString());
            }

            if (!string.IsNullOrWhiteSpace(filterCriteria.Position))
            {
                filtered = filtered.Where(c => c.Position != null &&
                    c.Position.Contains(filterCriteria.Position, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filterCriteria.Name))
            {
                filtered = filtered.Where(c => c.Name != null &&
                    c.Name.Contains(filterCriteria.Name, StringComparison.OrdinalIgnoreCase));
            }

            return filtered;
        }

        private IEnumerable<CandidateDto> ApplySorting(IEnumerable<CandidateDto> candidates, SortCriteria sortCriteria)
        {
            var isDescending = sortCriteria.SortOrder.ToLower() == "desc";

            return sortCriteria.SortBy.ToLower() switch
            {
                "name" => isDescending ?
                    candidates.OrderByDescending(c => c.Name) :
                    candidates.OrderBy(c => c.Name),

                "party" => isDescending ?
                    candidates.OrderByDescending(c => c.Party) :
                    candidates.OrderBy(c => c.Party),

                "position" => isDescending ?
                    candidates.OrderByDescending(c => c.Position) :
                    candidates.OrderBy(c => c.Position),

                "electionyear" => isDescending ?
                    candidates.OrderByDescending(c => int.TryParse(c.ElectionYear, out var year) ? year : 0) :
                    candidates.OrderBy(c => int.TryParse(c.ElectionYear, out var year) ? year : 0),

                "totalvotes" => isDescending ?
                    candidates.OrderByDescending(c => c.TotalVotes) :
                    candidates.OrderBy(c => c.TotalVotes),

                _ => candidates.OrderBy(c => c.Name)
            };
        }

        public async Task<List<CandidateDto>> GetAllCandidatesWithVotesAsync()
        {
            var candidates = await _candidateRepository.GetAllCandidatesWithVotesAsync();
            return MapCandidatesToDtos(candidates);
        }

        public async Task<CandidateDto> GetCandidateWithVotesAsync(int id)
        {
            var candidate = await _candidateRepository.GetCandidateWithVotesAsync(id);
            if (candidate == null)
                return null;
            return MapCandidateToDto(candidate);
        }

        private List<CandidateDto> MapCandidatesToDtos(List<Candidate> candidates)
        {
            var candidateDtos = new List<CandidateDto>();
            foreach (var candidate in candidates)
            {
                candidateDtos.Add(MapCandidateToDto(candidate));
            }
            return candidateDtos;
        }

        private CandidateDto MapCandidateToDto(Candidate candidate)
        {
            var candidateDto = new CandidateDto
            {
                Id = candidate.Id,
                Name = candidate.Name,
                Party = candidate.Party,
                Position = candidate.Position,
                ElectionYear = candidate.ElectionYear,
                Biography = candidate.Biography,
                TotalVotes = candidate.Votes.Sum(v => v.Count),
                Votes = candidate.Votes.Select(v => new VoteDto
                {
                    Id = v.Id,
                    PollingStation = v.PollingStation,
                    Count = v.Count,
                    RecordedAt = v.RecordedAt
                }).ToList()
            };
            return candidateDto;
        }

        public async Task<CandidateDto> CreateCandidateAsync(CreateCandidateDto createCandidateDto)
        {
            var candidate = new Candidate
            {
                Name = createCandidateDto.Name,
                Party = createCandidateDto.Party,
                Position = createCandidateDto.Position,
                ElectionYear = createCandidateDto.ElectionYear,
                Biography = createCandidateDto.Biography
            };

            var createdCandidate = await _candidateRepository.CreateCandidateAsync(candidate);
            return new CandidateDto
            {
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