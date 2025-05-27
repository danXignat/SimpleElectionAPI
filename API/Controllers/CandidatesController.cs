using Core.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CandidatesController : ControllerBase
    {
        private readonly CandidateService _candidateService;

        public CandidatesController(CandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<CandidateDto>>> GetAllCandidates(
            [FromQuery] string? party = null,
            [FromQuery] int? electionYear = null,
            [FromQuery] string? position = null,
            [FromQuery] string? name = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "name",
            [FromQuery] string sortOrder = "asc")
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var validSortFields = new[] { "name", "party", "position", "electionyear", "totalvotes" };
            if (!validSortFields.Contains(sortBy.ToLower()))
                sortBy = "name";

            if (sortOrder.ToLower() != "desc")
                sortOrder = "asc";

            var filterCriteria = new CandidateFilterCriteria
            {
                Party = party,
                ElectionYear = electionYear,
                Position = position,
                Name = name
            };

            var paginationCriteria = new PaginationCriteria
            {
                Page = page,
                PageSize = pageSize
            };

            var sortCriteria = new SortCriteria
            {
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            var result = await _candidateService.GetFilteredCandidatesAsync(
                filterCriteria, paginationCriteria, sortCriteria);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CandidateDto>> GetCandidate(int id)
        {
            var candidate = await _candidateService.GetCandidateWithVotesAsync(id);
            if (candidate == null)
                return NotFound();
            return Ok(candidate);
        }

        [HttpPost]
        public async Task<ActionResult<CandidateDto>> CreateCandidate([FromBody] CreateCandidateDto createCandidateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var candidate = await _candidateService.CreateCandidateAsync(createCandidateDto);
            return CreatedAtAction(
                nameof(GetCandidate),
                new { id = candidate.Id },
                candidate);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<CandidateDto>> UpdateCandidate(int id, [FromBody] UpdateCandidateDto updateCandidateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedCandidate = await _candidateService.UpdateCandidateAsync(id, updateCandidateDto);
            if (updatedCandidate == null)
            {
                return NotFound(new { message = $"Candidate with ID {id} not found." });
            }

            return Ok(updatedCandidate);
        }
    }
}