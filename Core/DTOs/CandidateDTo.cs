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

    public class CandidateFilterCriteria
    {
        public string? Party { get; set; }
        public int? ElectionYear { get; set; }
        public string? Position { get; set; }
        public string? Name { get; set; }
    }

    public class PaginationCriteria
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SortCriteria
    {
        public string SortBy { get; set; } = "name";
        public string SortOrder { get; set; } = "asc";
    }

    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class UpdateCandidateDto
    {
        public string? Name { get; set; }
        public string? Party { get; set; }
        public string? Position { get; set; }
        public string? ElectionYear { get; set; }
        public string? Biography { get; set; }
    }
}
