using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RL.Backend.Commands;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlanProcedureUserController : ControllerBase
    {

        private readonly IMediator _mediator;
        private readonly RLContext _context;
        public PlanProcedureUserController(IMediator mediator, RLContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [EnableQuery]
        public IEnumerable<PlanProcedureUser> Get()
        {
            return _context.PlanProcedureUsers;
        }

        [HttpPost("AssignUser")]
        public async Task<IActionResult> AssignUser(AssignPlanProcedureUsersCommand command, CancellationToken token)
        {
            var response = await _mediator.Send(command, token);

            return response.ToActionResult();
        }

        [HttpDelete("RemoveUser")]
        public async Task<IActionResult> RemoveUser([FromQuery] RemovePlanProcedureUsersCommand command, CancellationToken token)
        {
            var response = await _mediator.Send(command, token);

            return response.ToActionResult();
        }

        [HttpDelete("RemoveAllUsers")]
        public async Task<IActionResult> RemoveAllUsers([FromQuery] RemoveAllPlanProcedureUsersCommand command, CancellationToken token)
        {
            var response = await _mediator.Send(command, token);

            return response.ToActionResult();
        }
    }
}
