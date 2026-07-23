using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Abstractions.Cqrs;
using Mistruna.Core.DependencyInjection;
using Xunit;

namespace Mistruna.Core.Tests.Core;

public sealed class RequestValidationBehaviorTests
{
    [Fact]
    public async Task ValidationBehavior_Throws_ValidationException_WhenInvalid()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMistrunaCore(options =>
        {
            options.RegisterAssemblies(typeof(PingCommand).Assembly);
            options.AddValidation();
        });

        await using var serviceProvider = services.BuildServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        await Assert.ThrowsAsync<ValidationException>(
            () => mediator.Send(new PingCommand()));
    }

    public sealed class PingCommand : ICommand<string>
    {
        public string? Name { get; init; }
    }

    public sealed class PingValidator : AbstractValidator<PingCommand>
    {
        public PingValidator() => RuleFor(command => command.Name).NotEmpty();
    }

    public sealed class PingHandler : IRequestHandler<PingCommand, string>
    {
        public Task<string> Handle(PingCommand request, CancellationToken cancellationToken)
            => Task.FromResult(request.Name ?? string.Empty);
    }
}
