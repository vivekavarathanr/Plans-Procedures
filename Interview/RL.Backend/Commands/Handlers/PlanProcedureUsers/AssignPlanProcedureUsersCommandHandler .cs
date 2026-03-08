using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers
{
    public class AssignPlanProcedureUsersCommandHandler : IRequestHandler<AssignPlanProcedureUsersCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public AssignPlanProcedureUsersCommandHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(AssignPlanProcedureUsersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.PlanId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
                if (request.ProcedureId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
                if (request.UserId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid UserId"));

                var planProcedure = await _context.PlanProcedures
                    .FirstOrDefaultAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId, cancellationToken);
                if (planProcedure is null)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} with ProcedureId: {request.ProcedureId} not found"));

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId, cancellationToken);
                if (user is null)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"UserId: {request.UserId} not found"));

                var alreadyAssigned = await _context.PlanProcedureUsers
                    .AnyAsync(ppu => ppu.PlanId == request.PlanId && ppu.ProcedureId == request.ProcedureId && ppu.UserId == request.UserId, cancellationToken);
                if (alreadyAssigned)
                    return ApiResponse<Unit>.Succeed(new Unit());

                _context.PlanProcedureUsers.Add(new PlanProcedureUser
                {
                    PlanId = request.PlanId,
                    ProcedureId = request.ProcedureId,
                    UserId = request.UserId
                });

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
