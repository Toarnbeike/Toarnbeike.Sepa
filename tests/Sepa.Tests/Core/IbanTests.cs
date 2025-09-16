using Shouldly;
using Toarnbeike.Sepa.Core;

namespace Toarnbeike.Sepa.Tests.Core;

public class IbanTests
{
    [Fact]
    public void Create_Should_CreateIban_WhenInputIsValid()
    {
        var input = "GB94BARC10201530093459";
        var result = Iban.Create(input);
        result.ToString().ShouldBe(input);
    }
}