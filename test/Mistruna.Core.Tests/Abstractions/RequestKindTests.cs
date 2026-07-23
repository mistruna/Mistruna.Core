using Mistruna.Core.Abstractions.Cqrs;
using Xunit;

namespace Mistruna.Core.Tests.Abstractions;

public class RequestKindTests
{
    private sealed class SampleCommand : ICommand<int>;

    private sealed class SampleQuery : IQuery<string>;

    private sealed class PlainRequest;

    [Fact]
    public void IsCommand_returns_true_for_ICommand()
    {
        Assert.True(RequestKind.IsCommand(typeof(SampleCommand)));
    }

    [Fact]
    public void IsQuery_returns_true_for_IQuery()
    {
        Assert.True(RequestKind.IsQuery(typeof(SampleQuery)));
    }

    [Fact]
    public void IsCommand_returns_false_for_plain_request()
    {
        Assert.False(RequestKind.IsCommand(typeof(PlainRequest)));
    }

    [Fact]
    public void IsQuery_returns_false_for_command()
    {
        Assert.False(RequestKind.IsQuery(typeof(SampleCommand)));
    }
}
