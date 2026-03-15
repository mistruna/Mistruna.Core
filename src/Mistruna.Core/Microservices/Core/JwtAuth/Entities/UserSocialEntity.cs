using Mistruna.Core.Contracts.Base.Infrastructure;

namespace Mistruna.Core.Microservices.Core.JwtAuth.Entities;

public sealed class UserSocialEntity : IEntity
{
    #region IEntity

    public Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    #endregion

    public string? Site { get; set; }

    public string? FacebookLink { get; set; }

    public string? TwitterLink { get; set; }

    public string? InstagramLink { get; set; }

    public string? SkypeLink { get; set; }

    public string? LinkedIn { get; set; }

    public string? VkLink { get; set; }
}
