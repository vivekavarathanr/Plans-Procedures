using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.PlanProcedureUsers
{
    public class RemovePlanProcedureUsersCommandHandler : IRequestHandler<RemovePlanProcedureUsersCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public RemovePlanProcedureUsersCommandHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(RemovePlanProcedureUsersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.PlanId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
                if (request.ProcedureId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
                if (request.UserId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid UserId"));

                var record = await _context.PlanProcedureUsers.FirstOrDefaultAsync(
                    x => x.PlanId == request.PlanId && x.ProcedureId == request.ProcedureId && x.UserId == request.UserId, cancellationToken);

                if (record is null)
                    return ApiResponse<Unit>.Fail(new NotFoundException("No record is required to remove"));

                _context.PlanProcedureUsers.Remove(record);
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
