using Microsoft.AspNetCore.Mvc;
using uAccountPlan.Application.Interfaces;
using uAccountPlan.Domain.Entities;

namespace uAccountPlan.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountPlansController : ControllerBase
    {
        private readonly IAccountPlanService _service;

        public AccountPlansController(IAccountPlanService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AccountPlanDto accountPlanDto)
        {
            await _service.AddAsync(accountPlanDto);
            return CreatedAtAction(nameof(GetById), new { id = Guid.NewGuid() }, accountPlanDto);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("suggest-next-code/{parentId}")]
        public async Task<IActionResult> SuggestNextCode(Guid parentId)
        {
            var result = await _service.SuggestNextCodeAsync(parentId);
            return Ok(result);
        }
    }
}