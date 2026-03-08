using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers
{
    public class RemoveAllPlanProcedureUsersCommandHandler : IRequestHandler<RemoveAllPlanProcedureUsersCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public RemoveAllPlanProcedureUsersCommandHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(RemoveAllPlanProcedureUsersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.PlanId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
                if (request.ProcedureId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

                var records = await _context.PlanProcedureUsers
                    .Where(ppu => ppu.PlanId == request.PlanId && ppu.ProcedureId == request.ProcedureId)
                    .ToListAsync(cancellationToken);

                if (!records.Any())
                    return ApiResponse<Unit>.Succeed(new Unit());

                _context.PlanProcedureUsers.RemoveRange(records);
                await _context.SaveChangesAsync(cancellationToken);

                return ApiResponse<Unit>.Succeed(new Unit());
            }
            catch (Exception e)
            {
                return ApiResponse<Unit>.Fail(e);
            }
        }
    }
}
