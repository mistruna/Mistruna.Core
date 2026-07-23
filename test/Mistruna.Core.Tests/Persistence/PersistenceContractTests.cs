using FluentAssertions;
using Mistruna.Core.Abstractions.Persistence;
using Mistruna.Core.EfCore;
using Xunit;

namespace Mistruna.Core.Tests.Persistence;

public sealed class PersistenceContractTests
{
    [Fact]
    public void GenericRepository_ShouldComposeReadAndWriteContracts()
    {
        typeof(IReadRepository<TestEntity>)
            .IsAssignableFrom(typeof(IGenericRepository<TestEntity>))
            .Should()
            .BeTrue();

        typeof(IWriteRepository<TestEntity>)
            .IsAssignableFrom(typeof(IGenericRepository<TestEntity>))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void ReadRepository_ShouldNotExposeWriteMembers()
    {
        var methodNames = typeof(IReadRepository<TestEntity>)
            .GetMethods()
            .Select(method => method.Name);

        methodNames.Should().NotContain([
            "AddAsync",
            "AddRangeAsync",
            "UpdateAsync",
            "UpdateRangeAsync",
            "DeleteAsync",
            "DeleteRangeAsync"
        ]);
    }

    [Fact]
    public void WriteRepository_ShouldNotExposeReadMembers()
    {
        var methodNames = typeof(IWriteRepository<TestEntity>)
            .GetMethods()
            .Select(method => method.Name);

        methodNames.Should().NotContain([
            "GetByIdAsync",
            "GetAllAsync",
            "FindAsync",
            "FindOneAsync",
            "CountAsync",
            "AnyAsync",
            "AsQueryable"
        ]);
    }

    [Fact]
    public void UnitOfWork_ShouldReturnExplicitTransactionContract()
    {
        typeof(IUnitOfWork)
            .GetMethod(nameof(IUnitOfWork.BeginTransactionAsync))!
            .ReturnType
            .Should()
            .Be(typeof(Task<IUnitOfWorkTransaction>));
    }

    [Fact]
    public void LegacyDbTransactionContract_ShouldRemainCompatibleWithExplicitTransactionContract()
    {
        typeof(IUnitOfWorkTransaction)
            .IsAssignableFrom(typeof(IDbTransaction))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void EfCoreRepository_ShouldImplementAbstractionsContract()
    {
        typeof(IGenericRepository<TestEntity>)
            .IsAssignableFrom(typeof(EfGenericRepository<TestEntity>))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void EfCorePackage_ShouldNotDependOnAspNetCorePackage()
    {
        typeof(EfGenericRepository<>)
            .Assembly
            .GetReferencedAssemblies()
            .Should()
            .NotContain(reference => reference.Name == "Mistruna.Core.AspNetCore");
    }

    private sealed class TestEntity : IEntity
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
