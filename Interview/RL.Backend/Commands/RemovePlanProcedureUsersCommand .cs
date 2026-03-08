using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RL.Backend.Models;

namespace RL.Backend.Commands
{
    public class RemovePlanProcedureUsersCommand : IRequest<ApiResponse<Unit>>
    {
        public int PlanId { get; set; }
        public int ProcedureId { get; set; }
        public int UserId { get; set; }
    }
}
