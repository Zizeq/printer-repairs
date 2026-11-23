using System.Reflection;
using NetArchTest.Rules;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Domain.Aggregates;
using PrinterRepair.Diagnostics.Domain.Exceptions;
using PrinterRepair.Diagnostics.Infrastructure;
using Xunit;

namespace PrinterRepair.Diagnostics.Application.Tests.Architecture;

public class CleanArchitectureGuardTests
{
    private static readonly Assembly DomainAssembly = typeof(RepairTicket).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(IRepairTicketRepository).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(DependencyInjection).Assembly;

    [Fact]
    public void Domain_Should_Not_Depend_On_Application_Infrastructure_Or_EntityFramework()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "PrinterRepair.Diagnostics.Application",
                "PrinterRepair.Diagnostics.Infrastructure",
                "PrinterRepair.Diagnostics.Presentation",
                "Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful,
            $"Domain layer violates the Dependency Rule in types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Domain_Types_Should_Not_Have_Public_Property_Setters()
    {
        var typesWithPublicSetters = DomainAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsSubclassOf(typeof(Exception)))
            .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            .Where(p => p.SetMethod is not null && p.SetMethod.IsPublic)
            .Select(p => $"{p.DeclaringType?.Name}.{p.Name}")
            .ToList();

        Assert.Empty(typesWithPublicSetters);
    }

    [Fact]
    public void Domain_Exceptions_Should_Inherit_From_DomainException_And_Be_Sealed()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespaceMatching(@"\.Domain\.Exceptions")
            .And()
            .AreNotAbstract()
            .Should()
            .Inherit(typeof(DomainException))
            .And()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful,
            $"Invalid domain exceptions: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Infrastructure_Repositories_DbContext_And_Acl_Should_Not_Be_Public()
    {
        TestResult result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .HaveNameEndingWith("Repository")
            .Or()
            .HaveNameEndingWith("DbContext")
            .Or()
            .ResideInNamespaceMatching(@"\.Infrastructure\.Acl")
            .Should()
            .NotBePublic()
            .GetResult();

        Assert.True(result.IsSuccessful,
            $"Classes in Infrastructure must be marked internal: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Application_Ports_Should_Be_Interfaces_And_Not_Expose_Foreign_Dtos()
    {
        var leakedForeignDtos = new[] { DomainAssembly, ApplicationAssembly }
            .SelectMany(a => a.GetTypes())
            .Where(t => t.Name.EndsWith("_DTO", StringComparison.Ordinal))
            .Select(t => t.FullName)
            .ToList();

        Assert.Empty(leakedForeignDtos);
    }
}