using Core.DTOs;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class VotesController : ControllerBase {
        private readonly VoteService _voteService;

        public VotesController(VoteService voteService) {
            _voteService = voteService;
        }

        [HttpPost]
        public async Task<ActionResult<VoteDto>> AddVote([FromBody] CreateVoteDto createVoteDto) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var vote = await _voteService.AddVoteAsync(createVoteDto);
                return CreatedAtAction(nameof(AddVote), vote);
            }
            catch (ArgumentException ex) {
                return NotFound(ex.Message);
            }
            catch (Exception ex) {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<VoteDto>>> GetAllVotes() {
            var votes = await _voteService.GetAllVotesAsync();
            return Ok(votes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VoteDto>> GetVote(int id) {
            var vote = await _voteService.GetVoteByIdAsync(id);
            if (vote == null)
                return NotFound();

            return Ok(vote);
        }

        [HttpGet("candidate/{candidateId}")]
        public async Task<ActionResult<List<VoteDto>>> GetVotesByCandidate(int candidateId) {
            try {
                // Check if candidate exists
                var candidateExists = await _voteService.CandidateExistsAsync(candidateId);
                if (!candidateExists)
                    return NotFound($"Candidate with ID {candidateId} not found");

                var votes = await _voteService.GetVotesByCandidateIdAsync(candidateId);
                return Ok(votes);
            }
            catch (Exception ex) {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
