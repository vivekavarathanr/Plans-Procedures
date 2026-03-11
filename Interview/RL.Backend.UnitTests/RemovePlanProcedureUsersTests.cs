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
public class RemovePlanProcedureUsersTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task RemovePlanProcedureUsersTests_InvalidPlanId_ReturnsBadRequest(int planId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new RemovePlanProcedureUsersCommandHandler(context.Object);
        var request = new RemovePlanProcedureUsersCommand
        {
            PlanId = planId,
            ProcedureId = 1,
            UserId = 1
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
    public async Task RemovePlanProcedureUsersTests_InvalidProcedureId_ReturnsBadRequest(int procedureId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new RemovePlanProcedureUsersCommandHandler(context.Object);
        var request = new RemovePlanProcedureUsersCommand
        {
            PlanId = 1,
            ProcedureId = procedureId,
            UserId = 1
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
    public async Task RemovePlanProcedureUsersTests_InvalidUserId_ReturnsBadRequest(int userId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new RemovePlanProcedureUsersCommandHandler(context.Object);
        var request = new RemovePlanProcedureUsersCommand
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = userId
        };

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(BadRequestException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(19, 1010, 3)]
    [DataRow(35, 69, 22)]
    public async Task RemovePlanProcedureUsersTests_RecordNotFound_ReturnsNotFound(int planId, int procedureId, int userId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemovePlanProcedureUsersCommandHandler(context);
        var request = new RemovePlanProcedureUsersCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        context.PlanProcedureUsers.Add(new PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId + 1
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(19, 1010, 3)]
    [DataRow(35, 69, 22)]
    public async Task RemovePlanProcedureUsersTests_RemovesSingleRecord_ReturnsSuccess(int planId, int procedureId, int userId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new RemovePlanProcedureUsersCommandHandler(context);
        var request = new RemovePlanProcedureUsersCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        context.PlanProcedureUsers.AddRange(
            new PlanProcedureUser
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            },
            new PlanProcedureUser
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId + 1
            });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        var removedRecord = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
            ppu.PlanId == planId && ppu.ProcedureId == procedureId && ppu.UserId == userId);
        var untouchedRecord = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
            ppu.PlanId == planId && ppu.ProcedureId == procedureId && ppu.UserId == userId + 1);

        removedRecord.Should().BeNull();
        untouchedRecord.Should().NotBeNull();
        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }
}