using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.PlanProcedureUsers;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests;

[TestClass]
public class RemoveAllPlanProcedureUsersTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task RemoveAllPlanProcedureUsersTests_InvalidPlanId_ReturnsBadRequest(int planId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new RemoveAllPlanProcedureUsersCommandHandler(context.Object);
        var request = new RemoveAllPlanProcedureUsersCommand
        {
            PlanId = planId,
            ProcedureId = 1
        };

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task RemoveAllPlanProcedureUsersTests_InvalidProcedureId_ReturnsBadRequest(int procedureId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new RemoveAllPlanProcedureUsersCommandHandler(context.Object);
        var request = new RemoveAllPlanProcedureUsersCommand
        {
            PlanId = 1,
            ProcedureId = procedureId
        };

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(19, 1010)]
    [DataRow(35, 69)]
    public async Task RemoveAllPlanProcedureUsersTests_NoRecordsToRemove_ReturnsNotFound(int planId, int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveAllPlanProcedureUsersCommandHandler(context);
        var request = new RemoveAllPlanProcedureUsersCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        context.PlanProcedureUsers.Add(new PlanProcedureUser
        {
            PlanId = planId + 1,
            ProcedureId = procedureId + 1,
            UserId = 1
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(19, 1010)]
    [DataRow(35, 69)]
    public async Task RemoveAllPlanProcedureUsersTests_RemovesAllMatchingRecords_ReturnsSuccess(int planId, int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemoveAllPlanProcedureUsersCommandHandler(context);
        var request = new RemoveAllPlanProcedureUsersCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        context.PlanProcedureUsers.AddRange(
            new PlanProcedureUser
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = 1
            },
            new PlanProcedureUser
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = 2
            },
            new PlanProcedureUser
            {
                PlanId = planId,
                ProcedureId = procedureId + 1,
                UserId = 3
            });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        var removedRecords = await context.PlanProcedureUsers.Where(ppu => ppu.PlanId == planId && ppu.ProcedureId == procedureId).ToListAsync();
        var untouchedRecord = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
            ppu.PlanId == planId && ppu.ProcedureId == procedureId + 1 && ppu.UserId == 3);

        removedRecords.Should().BeEmpty();
        untouchedRecord.Should().NotBeNull();
        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }
}