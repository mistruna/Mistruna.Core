namespace Mistruna.Core.Base.ComplexTypes;

public class SystemUsersEnumeration(string value, Guid id) : TypeEnumeration(value)
{
    public static readonly SystemUsersEnumeration SystemMessage =
        new(nameof(SystemMessage), Guid.Parse("00000000-0000-0000-0001-000000000000"));

    private Guid Identifier { get; set; } = id;

    public override bool Equals(object obj) => Identifier.Equals(((SystemUsersEnumeration)obj)!.Identifier);
}
