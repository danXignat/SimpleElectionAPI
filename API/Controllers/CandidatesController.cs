using Core.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CandidatesController : ControllerBase {
        private readonly CandidateService _candidateService;

        public CandidatesController(CandidateService candidateService) {
            _candidateService = candidateService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CandidateDto>>> GetAllCandidates() {
            var candidates = await _candidateService.GetAllCandidatesWithVotesAsync();
            return Ok(candidates);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CandidateDto>> GetCandidate(int id) {
            var candidate = await _candidateService.GetCandidateWithVotesAsync(id);

            if (candidate == null)
                return NotFound();

            return Ok(candidate);
        }

        [HttpPost]
        public async Task<ActionResult<CandidateDto>> CreateCandidate([FromBody] CreateCandidateDto createCandidateDto) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var candidate = await _candidateService.CreateCandidateAsync(createCandidateDto);

            return CreatedAtAction(
                nameof(GetCandidate),
                new { id = candidate.Id },
                candidate);
        }
    }
}
