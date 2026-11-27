using System.Reflection;
using System.Runtime.CompilerServices;
using NetArchTest.Rules;
using PrinterRepair.Diagnostics.Application.Repositories;
using PrinterRepair.Diagnostics.Domain.Aggregates;
using PrinterRepair.Diagnostics.Domain.Exceptions;
using PrinterRepair.Diagnostics.Infrastructure;
using Xunit;

namespace PrinterRepair.Diagnostics.Application.Tests.Architecture;

public sealed class CleanArchitectureGuardTests
{
    private static readonly Assembly DomainAssembly = typeof(RepairTicket).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(IRepairTicketRepository).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(DependencyInjection).Assembly;

    [Fact]
    public void Domain_Should_Not_Depend_On_Application_Infrastructure_Or_Frameworks()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "PrinterRepair.Diagnostics.Application",
                "PrinterRepair.Diagnostics.Infrastructure",
                "PrinterRepair.Diagnostics.Presentation",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "System.Net.Http",
                "Confluent.Kafka")
            .GetResult();

        Assert.True(result.IsSuccessful,
            $"FF-01 violated! Domain layer has forbidden dependencies in types: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Domain_Types_Should_Not_Have_Public_Mutable_Property_Setters()
    {
        var typesWithPublicMutableSetters = DomainAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsSubclassOf(typeof(Exception)))
            .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            .Where(p => p.SetMethod is not null
                        && p.SetMethod.IsPublic
                        && !p.SetMethod.ReturnParameter
                            .GetRequiredCustomModifiers()
                            .Contains(typeof(IsExternalInit)))
            .Select(p => $"{p.DeclaringType?.Name}.{p.Name}")
            .ToList();

        Assert.Empty(typesWithPublicMutableSetters);
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
    public void Application_Should_Not_Depend_On_Infrastructure_Or_Presentation_And_Handlers_Should_Be_Internal_Sealed()
    {
        TestResult dependencyResult = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(
                "PrinterRepair.Diagnostics.Infrastructure",
                "PrinterRepair.Diagnostics.Presentation")
            .GetResult();

        Assert.True(dependencyResult.IsSuccessful,
            $"FF-03 violated! Application references outer layers in types: {string.Join(", ", dependencyResult.FailingTypeNames ?? [])}");

        TestResult handlerResult = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("CommandHandler")
            .Or()
            .HaveNameEndingWith("QueryHandler")
            .Should()
            .NotBePublic()
            .And()
            .BeSealed()
            .GetResult();

        Assert.True(handlerResult.IsSuccessful,
            $"FF-03 violated! CQRS handlers must be internal sealed: {string.Join(", ", handlerResult.FailingTypeNames ?? [])}");
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
            $"FF-04 violated! Infrastructure persistence and ACL types must be internal: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Domain_And_Application_Should_Not_Expose_Foreign_Dtos_Or_Depend_On_Acl()
    {
        var leakedForeignDtos = new[] { DomainAssembly, ApplicationAssembly }
            .SelectMany(a => a.GetTypes())
            .Where(t => t.Name.EndsWith("_DTO", StringComparison.Ordinal))
            .Select(t => t.FullName)
            .ToList();

        Assert.Empty(leakedForeignDtos);

        TestResult aclDependencyResult = Types.InAssemblies([DomainAssembly, ApplicationAssembly])
            .ShouldNot()
            .HaveDependencyOn("PrinterRepair.Diagnostics.Infrastructure.Acl")
            .GetResult();

        Assert.True(aclDependencyResult.IsSuccessful,
            $"FF-04 violated! ACL types leaked into inner layers: {string.Join(", ", aclDependencyResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Diagnostics_Microservice_Should_Not_Reference_Other_Microservices()
    {
        TestResult result = Types.InAssemblies([DomainAssembly, ApplicationAssembly, InfrastructureAssembly])
            .ShouldNot()
            .HaveDependencyOnAny(
                "PrinterRepair.BAH",
                "PrinterRepair.Identification")
            .GetResult();

        Assert.True(result.IsSuccessful,
            $"FF-05 violated! Cross-microservice dependency detected: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}