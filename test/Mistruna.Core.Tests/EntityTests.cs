using FluentAssertions;
using Mistruna.Core.Contracts.Base.Entities;
using Xunit;

namespace Mistruna.Core.Tests;

public class EntityTests
{
    #region Entity Tests

    private class TestEntity : Entity<Guid>
    {
        public TestEntity(Guid id) { Id = id; }
    }

    [Fact]
    public void Entity_WithSameId_ShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        entity1.Should().Be(entity2);
        (entity1 == entity2).Should().BeTrue();
    }

    [Fact]
    public void Entity_WithDifferentId_ShouldNotBeEqual()
    {
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        entity1.Should().NotBe(entity2);
        (entity1 != entity2).Should().BeTrue();
    }

    [Fact]
    public void Entity_WithDefaultId_ShouldNotBeEqual()
    {
        var entity1 = new TestEntity(default);
        var entity2 = new TestEntity(default);

        entity1.Equals(entity2).Should().BeFalse();
    }

    #endregion

    #region ValueObject Tests

    private class TestValueObject : ValueObject
    {
        public string Value1 { get; }
        public int Value2 { get; }

        public TestValueObject(string value1, int value2)
        {
            Value1 = value1;
            Value2 = value2;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value1;
            yield return Value2;
        }
    }

    [Fact]
    public void ValueObject_WithSameValues_ShouldBeEqual()
    {
        var vo1 = new TestValueObject("test", 42);
        var vo2 = new TestValueObject("test", 42);

        vo1.Should().Be(vo2);
        (vo1 == vo2).Should().BeTrue();
    }

    [Fact]
    public void ValueObject_WithDifferentValues_ShouldNotBeEqual()
    {
        var vo1 = new TestValueObject("test1", 42);
        var vo2 = new TestValueObject("test2", 42);

        vo1.Should().NotBe(vo2);
        (vo1 != vo2).Should().BeTrue();
    }

    #endregion

    #region Email Tests

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.org")]
    [InlineData("user+tag@example.co.uk")]
    public void Email_ValidEmail_ShouldCreate(string emailStr)
    {
        var email = Email.Create(emailStr);

        email.Value.Should().Be(emailStr.ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    public void Email_InvalidEmail_ShouldThrow(string emailStr)
    {
        var action = () => Email.Create(emailStr);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Email_TryCreate_WithValidEmail_ShouldReturnTrue()
    {
        var result = Email.TryCreate("test@example.com", out var email);

        result.Should().BeTrue();
        email.Should().NotBeNull();
    }

    [Fact]
    public void Email_TryCreate_WithInvalidEmail_ShouldReturnFalse()
    {
        var result = Email.TryCreate("invalid", out var email);

        result.Should().BeFalse();
        email.Should().BeNull();
    }

    #endregion

    #region PhoneNumber Tests

    [Theory]
    [InlineData("+1234567890", "+1234567890")]
    [InlineData("+7 (999) 123-45-67", "+79991234567")]
    [InlineData("1234567890", "1234567890")]
    public void PhoneNumber_ValidPhone_ShouldCreate(string input, string expected)
    {
        var phone = PhoneNumber.Create(input);

        phone.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    public void PhoneNumber_InvalidPhone_ShouldThrow(string input)
    {
        var action = () => PhoneNumber.Create(input);

        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Money Tests

    [Fact]
    public void Money_Create_ShouldRoundToTwoDecimals()
    {
        var money = Money.Create(10.555m, "USD");

        money.Amount.Should().Be(10.56m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Money_Add_SameCurrency_ShouldAdd()
    {
        var m1 = Money.Create(10, "USD");
        var m2 = Money.Create(20, "USD");

        var result = m1 + m2;

        result.Amount.Should().Be(30);
    }

    [Fact]
    public void Money_Add_DifferentCurrency_ShouldThrow()
    {
        var m1 = Money.Create(10, "USD");
        var m2 = Money.Create(20, "EUR");

        var action = () => { var _ = m1 + m2; };

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Money_Multiply_ShouldMultiply()
    {
        var money = Money.Create(10, "USD");

        var result = money * 3;

        result.Amount.Should().Be(30);
    }

    [Fact]
    public void Money_Comparison_SameCurrency_ShouldCompare()
    {
        var m1 = Money.Create(10, "USD");
        var m2 = Money.Create(20, "USD");

        (m1 < m2).Should().BeTrue();
        (m2 > m1).Should().BeTrue();
        (m1 <= m2).Should().BeTrue();
        (m2 >= m1).Should().BeTrue();
    }

    [Fact]
    public void Money_Zero_ShouldCreateZero()
    {
        var money = Money.Zero("EUR");

        money.Amount.Should().Be(0);
        money.Currency.Should().Be("EUR");
    }

    #endregion

    #region Address Tests

    [Fact]
    public void Address_Create_ShouldCreateValid()
    {
        var address = Address.Create("123 Main St", "New York", "10001", "USA", "NY");

        address.Street.Should().Be("123 Main St");
        address.City.Should().Be("New York");
        address.State.Should().Be("NY");
        address.PostalCode.Should().Be("10001");
        address.Country.Should().Be("USA");
    }

    [Fact]
    public void Address_Create_WithoutState_ShouldCreateValid()
    {
        var address = Address.Create("123 Main St", "London", "SW1A 1AA", "UK");

        address.State.Should().BeNull();
    }

    [Fact]
    public void Address_ToString_ShouldFormat()
    {
        var address = Address.Create("123 Main St", "New York", "10001", "USA", "NY");

        address.ToString().Should().Contain("123 Main St");
        address.ToString().Should().Contain("New York");
    }

    #endregion

    #region DateRange Tests

    [Fact]
    public void DateRange_Create_ValidRange_ShouldCreate()
    {
        var start = DateTime.Today;
        var end = DateTime.Today.AddDays(7);

        var range = DateRange.Create(start, end);

        range.Start.Should().Be(start);
        range.End.Should().Be(end);
        range.Duration.Should().Be(TimeSpan.FromDays(7));
    }

    [Fact]
    public void DateRange_Create_EndBeforeStart_ShouldThrow()
    {
        var start = DateTime.Today;
        var end = DateTime.Today.AddDays(-1);

        var action = () => DateRange.Create(start, end);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DateRange_Contains_ShouldCheckDate()
    {
        var range = DateRange.Create(
            new DateTime(2024, 1, 1),
            new DateTime(2024, 12, 31));

        range.Contains(new DateTime(2024, 6, 15)).Should().BeTrue();
        range.Contains(new DateTime(2023, 6, 15)).Should().BeFalse();
    }

    [Fact]
    public void DateRange_Overlaps_ShouldCheckOverlap()
    {
        var range1 = DateRange.Create(
            new DateTime(2024, 1, 1),
            new DateTime(2024, 6, 30));

        var range2 = DateRange.Create(
            new DateTime(2024, 6, 1),
            new DateTime(2024, 12, 31));

        var range3 = DateRange.Create(
            new DateTime(2024, 7, 1),
            new DateTime(2024, 12, 31));

        range1.Overlaps(range2).Should().BeTrue();
        range1.Overlaps(range3).Should().BeFalse();
    }

    #endregion

    #region SoftDeletable Tests

    private class TestSoftDeletable : SoftDeletableEntity
    {
        public TestSoftDeletable() : base() { }
    }

    [Fact]
    public void SoftDeletable_Delete_ShouldMarkDeleted()
    {
        var entity = new TestSoftDeletable();

        entity.Delete("testuser");

        entity.IsDeleted.Should().BeTrue();
        entity.DeletedBy.Should().Be("testuser");
        entity.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public void SoftDeletable_Restore_ShouldUndelete()
    {
        var entity = new TestSoftDeletable();
        entity.Delete("testuser");

        entity.Restore();

        entity.IsDeleted.Should().BeFalse();
        entity.DeletedBy.Should().BeNull();
        entity.DeletedAt.Should().BeNull();
    }

    #endregion
}
