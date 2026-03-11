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
public class AssignPlanProcedureUsersTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task AssignPlanProcedureUsersTests_InvalidPlanId_ReturnsBadRequest(int planId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new AssignPlanProcedureUsersCommandHandler(context.Object);
        var request = new AssignPlanProcedureUsersCommand
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
    public async Task AssignPlanProcedureUsersTests_InvalidProcedureId_ReturnsBadRequest(int procedureId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new AssignPlanProcedureUsersCommandHandler(context.Object);
        var request = new AssignPlanProcedureUsersCommand
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
    public async Task AssignPlanProcedureUsersTests_InvalidUserId_ReturnsBadRequest(int userId)
    {
        //Given
        var context = new Mock<RLContext>();
        var sut = new AssignPlanProcedureUsersCommandHandler(context.Object);
        var request = new AssignPlanProcedureUsersCommand
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
    [DataRow(1, 1)]
    [DataRow(19, 1010)]
    [DataRow(35, 69)]
    public async Task AssignPlanProcedureUsersTests_PlanProcedureNotFound_ReturnsNotFound(int planId, int procedureId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AssignPlanProcedureUsersCommandHandler(context);
        var request = new AssignPlanProcedureUsersCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = 1
        };

        context.PlanProcedures.Add(new PlanProcedure
        {
            PlanId = planId + 1,
            ProcedureId = procedureId + 1
        });
        context.Users.Add(new User
        {
            UserId = 1,
            Name = "Test User"
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Exception.Should().BeOfType(typeof(NotFoundException));
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AssignPlanProcedureUsersTests_UserNotFound_ReturnsNotFound(int userId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AssignPlanProcedureUsersCommandHandler(context);
        var request = new AssignPlanProcedureUsersCommand
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = userId
        };

        context.PlanProcedures.Add(new PlanProcedure
        {
            PlanId = 1,
            ProcedureId = 1
        });
        context.Users.Add(new User
        {
            UserId = userId + 1,
            Name = "Test User"
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
    public async Task AssignPlanProcedureUsersTests_AlreadyAssigned_ReturnsSuccess(int planId, int procedureId, int userId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AssignPlanProcedureUsersCommandHandler(context);
        var request = new AssignPlanProcedureUsersCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        context.PlanProcedures.Add(new PlanProcedure
        {
            PlanId = planId,
            ProcedureId = procedureId
        });
        context.Users.Add(new User
        {
            UserId = userId,
            Name = "Test User"
        });
        context.PlanProcedureUsers.Add(new PlanProcedureUser
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(19, 1010, 3)]
    [DataRow(35, 69, 22)]
    public async Task AssignPlanProcedureUsersTests_AssignsUser_ReturnsSuccess(int planId, int procedureId, int userId)
    {
        //Given
        var context = DbContextHelper.CreateContext();
        var sut = new AssignPlanProcedureUsersCommandHandler(context);
        var request = new AssignPlanProcedureUsersCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        context.PlanProcedures.Add(new PlanProcedure
        {
            PlanId = planId,
            ProcedureId = procedureId
        });
        context.Users.Add(new User
        {
            UserId = userId,
            Name = "Test User"
        });
        await context.SaveChangesAsync();

        //When
        var result = await sut.Handle(request, new CancellationToken());

        //Then
        var dbRecord = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
            ppu.PlanId == planId && ppu.ProcedureId == procedureId && ppu.UserId == userId);

        dbRecord.Should().NotBeNull();
        result.Value.Should().BeOfType(typeof(Unit));
        result.Succeeded.Should().BeTrue();
    }
}