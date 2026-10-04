using System.Reflection;
using NetArchTest.Rules;

namespace TwoGether.ArchitectureTests;

public class LayerDependencyTests
{
    private const string Domain = "TwoGether.Domain";
    private const string Application = "TwoGether.Application";
    private const string Infrastructure = "TwoGether.Infrastructure";
    private const string Persistence = "TwoGether.Persistence";
    private const string Api = "TwoGether.Api";

    private static readonly Assembly DomainAssembly = typeof(Domain.Common.Entity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(Application.DependencyInjection).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.DependencyInjection).Assembly;
    private static readonly Assembly PersistenceAssembly = typeof(Persistence.DependencyInjection).Assembly;

    [Fact]
    public void Domain_depends_only_on_geometry_library()
    {
        var result = Types.InAssembly(DomainAssembly).ShouldNot()
            .HaveDependencyOnAny(Application, Infrastructure, Persistence, Api,
                "Microsoft.EntityFrameworkCore", "Npgsql", "FluentValidation")
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Application_does_not_depend_on_outer_layers_or_db_provider()
    {
        var result = Types.InAssembly(ApplicationAssembly).ShouldNot()
            .HaveDependencyOnAny(Infrastructure, Persistence, Api, "Npgsql")
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Infrastructure_and_persistence_do_not_depend_on_api_or_each_other()
    {
        var infrastructure = Types.InAssembly(InfrastructureAssembly).ShouldNot()
            .HaveDependencyOnAny(Persistence, Api).GetResult();
        var persistence = Types.InAssembly(PersistenceAssembly).ShouldNot()
            .HaveDependencyOnAny(Infrastructure, Api).GetResult();

        infrastructure.FailingTypeNames.Should().BeNullOrEmpty();
        persistence.FailingTypeNames.Should().BeNullOrEmpty();
    }

    [Fact]
    public void Persistence_implementations_are_internal()
    {
        var result = Types.InAssembly(PersistenceAssembly)
            .That().ResideInNamespaceStartingWith($"{Persistence}.Configurations")
            .Should().NotBePublic()
            .GetResult();

        result.FailingTypeNames.Should().BeNullOrEmpty();
    }
}
