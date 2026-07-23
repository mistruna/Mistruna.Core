using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Abstractions.Persistence;
using Mistruna.Core.EfCore;
using Xunit;

namespace Mistruna.Core.Tests.Persistence;

public sealed class UnitOfWorkTests
{
    [Fact]
    public async Task RepositoryAddAsync_ShouldNotPersistUntilUnitOfWorkSaves()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var repository = new TestEntityRepository(context);

        await repository.AddAsync(new TestEntity
        {
            Id = Guid.NewGuid(),
            Name = "pending"
        });

        await using var verificationContext = database.CreateContext();
        var existsBeforeSave = await verificationContext.Entities.AnyAsync();

        existsBeforeSave.Should().BeFalse();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistPendingRepositoryChanges()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var repository = new TestEntityRepository(context);
        var unitOfWork = new EfUnitOfWork<TestDbContext>(context);

        await repository.AddAsync(new TestEntity
        {
            Id = Guid.NewGuid(),
            Name = "saved"
        });
        await unitOfWork.SaveChangesAsync();

        await using var verificationContext = database.CreateContext();
        var saved = await verificationContext.Entities.SingleAsync();

        saved.Name.Should().Be("saved");
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_ShouldPersistChangesWhenActionSucceeds()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var repository = new TestEntityRepository(context);
        var unitOfWork = new EfUnitOfWork<TestDbContext>(context);

        await unitOfWork.ExecuteInTransactionAsync(async cancellationToken =>
        {
            await repository.AddAsync(new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "committed"
            }, cancellationToken);
        });

        await using var verificationContext = database.CreateContext();
        var saved = await verificationContext.Entities.SingleAsync();

        saved.Name.Should().Be("committed");
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_ShouldRunActionInsideConfiguredExecutionStrategy()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext(useTrackingExecutionStrategy: true);
        var unitOfWork = new EfUnitOfWork<TestDbContext>(context);
        var executedInsideStrategy = false;

        await unitOfWork.ExecuteInTransactionAsync(_ =>
        {
            executedInsideStrategy = TrackingExecutionStrategy.IsExecuting.Value;
            return Task.CompletedTask;
        });

        executedInsideStrategy.Should().BeTrue();
    }
    [Fact]
    public async Task ExecuteInTransactionAsync_ShouldRollbackSavedChangesWhenActionThrows()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var repository = new TestEntityRepository(context);
        var unitOfWork = new EfUnitOfWork<TestDbContext>(context);

        var action = () => unitOfWork.ExecuteInTransactionAsync(async cancellationToken =>
        {
            await repository.AddAsync(new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "rolled-back"
            }, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            throw new InvalidOperationException("Simulated failure.");
        });

        await action.Should().ThrowAsync<InvalidOperationException>();
        await using var verificationContext = database.CreateContext();
        var existsAfterRollback = await verificationContext.Entities.AnyAsync();

        existsAfterRollback.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_ShouldThrowArgumentNullExceptionWhenActionIsNull()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var unitOfWork = new EfUnitOfWork<TestDbContext>(context);
        Func<CancellationToken, Task> action = null!;

        var act = () => unitOfWork.ExecuteInTransactionAsync(action);

        await act.Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName("action");
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_ShouldRollbackWhenRequestTokenIsCanceledAfterPartialSave()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var repository = new TestEntityRepository(context);
        var unitOfWork = new EfUnitOfWork<TestDbContext>(context);
        using var cancellationTokenSource = new CancellationTokenSource();

        var action = () => unitOfWork.ExecuteInTransactionAsync(async cancellationToken =>
        {
            await repository.AddAsync(new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "rollback-after-cancel"
            }, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await cancellationTokenSource.CancelAsync();

            throw new InvalidOperationException("Simulated failure after request cancellation.");
        }, cancellationTokenSource.Token);

        await action.Should().ThrowAsync<InvalidOperationException>();
        await using var verificationContext = database.CreateContext();
        var existsAfterRollback = await verificationContext.Entities.AnyAsync();

        existsAfterRollback.Should().BeFalse();
    }
    [Fact]
    public async Task BeginTransactionAsync_ShouldRollbackSavedChangesWhenDisposedWithoutCommit()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var repository = new TestEntityRepository(context);
        var unitOfWork = new EfUnitOfWork<TestDbContext>(context);

        await using (await unitOfWork.BeginTransactionAsync())
        {
            await repository.AddAsync(new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "disposed"
            });
            await unitOfWork.SaveChangesAsync();
        }

        await using var verificationContext = database.CreateContext();
        var existsAfterDispose = await verificationContext.Entities.AnyAsync();

        existsAfterDispose.Should().BeFalse();
    }

    [Fact]
    public async Task BeginTransactionAsync_ShouldRollbackSavedChangesWhenSynchronouslyDisposedWithoutCommit()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var repository = new TestEntityRepository(context);
        var unitOfWork = new EfUnitOfWork<TestDbContext>(context);

        using (await unitOfWork.BeginTransactionAsync())
        {
            await repository.AddAsync(new TestEntity
            {
                Id = Guid.NewGuid(),
                Name = "sync-disposed"
            });
            await unitOfWork.SaveChangesAsync();
        }

        await using var verificationContext = database.CreateContext();
        var existsAfterDispose = await verificationContext.Entities.AnyAsync();

        existsAfterDispose.Should().BeFalse();
    }

    [Fact]
    public async Task BeginTransactionAsync_ShouldRejectNestedTransactions()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var unitOfWork = new EfUnitOfWork<TestDbContext>(context);

        await using var _ = await unitOfWork.BeginTransactionAsync();
        var action = () => unitOfWork.BeginTransactionAsync();

        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*transaction*active*");
    }

    [Fact]
    public void GenericRepositoryContract_ShouldNotExposeSaveChanges()
    {
        typeof(IGenericRepository<TestEntity>)
            .GetMethods()
            .Should()
            .NotContain(method => method.Name == "SaveChangesAsync");
    }

    [Fact]
    public void UnitOfWorkContracts_ShouldSupportAsyncDisposal()
    {
        typeof(IAsyncDisposable).IsAssignableFrom(typeof(IUnitOfWork)).Should().BeTrue();
        typeof(IAsyncDisposable).IsAssignableFrom(typeof(IUnitOfWorkTransaction)).Should().BeTrue();
    }

    [Fact]
    public async Task Dispose_ShouldNotDisposeInjectedDbContext()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var unitOfWork = new EfUnitOfWork<TestDbContext>(context);

        unitOfWork.Dispose();

        var action = () => context.Entities.Any();
        action.Should().NotThrow();
    }

    [Fact]
    public async Task DisposeAsync_ShouldNotDisposeInjectedDbContext()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var unitOfWork = new EfUnitOfWork<TestDbContext>(context);

        await unitOfWork.DisposeAsync();

        var action = () => context.Entities.AnyAsync();
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AddMistrunaEfCore_ShouldRegisterScopedPersistenceServices()
    {
        await using var database = await TestDatabase.CreateAsync();
        var services = new ServiceCollection();
        services.AddDbContext<TestDbContext>(options => options.UseSqlite(database.Connection));

        services.AddMistrunaEfCore<TestDbContext>();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var repository = scope.ServiceProvider.GetRequiredService<IGenericRepository<TestEntity>>();

        unitOfWork.Should().BeOfType<EfUnitOfWork<TestDbContext>>();
        repository.Should().BeOfType<EfGenericRepository<TestEntity>>();
    }

    [Fact]
    public async Task RegisteredServices_ShouldPersistChangesWithEfInMemory()
    {
        var services = new ServiceCollection();
        services.AddDbContext<TestDbContext>(
            options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddMistrunaEfCore<TestDbContext>();

        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IGenericRepository<TestEntity>>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        await repository.AddAsync(new TestEntity { Id = Guid.NewGuid(), Name = "in-memory" });
        await unitOfWork.SaveChangesAsync();

        var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
        (await context.Entities.SingleAsync()).Name.Should().Be("in-memory");
    }

    private sealed class TestDatabase : IAsyncDisposable
    {
        private TestDatabase(SqliteConnection connection)
        {
            Connection = connection;
        }

        public SqliteConnection Connection { get; }

        public static async Task<TestDatabase> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            var database = new TestDatabase(connection);
            await using var context = database.CreateContext();
            await context.Database.EnsureCreatedAsync();

            return database;
        }

        public TestDbContext CreateContext(bool useTrackingExecutionStrategy = false)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            optionsBuilder.UseSqlite(Connection, sqliteOptions =>
            {
                if (useTrackingExecutionStrategy)
                {
                    sqliteOptions.ExecutionStrategy(dependencies => new TrackingExecutionStrategy(dependencies));
                }
            });

            return new TestDbContext(optionsBuilder.Options);
        }

        public async ValueTask DisposeAsync()
        {
            await Connection.DisposeAsync();
        }
    }

    private sealed class TrackingExecutionStrategy(ExecutionStrategyDependencies dependencies) : IExecutionStrategy
    {
        public static readonly AsyncLocal<bool> IsExecuting = new();

        public bool RetriesOnFailure => false;

        public TResult Execute<TState, TResult>(
            TState state,
            Func<DbContext, TState, TResult> operation,
            Func<DbContext, TState, ExecutionResult<TResult>>? verifySucceeded)
        {
            var previous = IsExecuting.Value;
            IsExecuting.Value = true;
            try
            {
                return operation(dependencies.CurrentContext.Context, state);
            }
            finally
            {
                IsExecuting.Value = previous;
            }
        }

        public async Task<TResult> ExecuteAsync<TState, TResult>(
            TState state,
            Func<DbContext, TState, CancellationToken, Task<TResult>> operation,
            Func<DbContext, TState, CancellationToken, Task<ExecutionResult<TResult>>>? verifySucceeded,
            CancellationToken cancellationToken = default)
        {
            var previous = IsExecuting.Value;
            IsExecuting.Value = true;
            try
            {
                return await operation(dependencies.CurrentContext.Context, state, cancellationToken);
            }
            finally
            {
                IsExecuting.Value = previous;
            }
        }
    }
    private sealed class TestEntity : IEntity
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<TestEntity> Entities => Set<TestEntity>();
    }

    private sealed class TestEntityRepository(TestDbContext context)
        : EfGenericRepository<TestEntity>(context);
}
