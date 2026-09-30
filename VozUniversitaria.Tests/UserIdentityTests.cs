using Microsoft.AspNetCore.Identity;
using Xunit;

public class UserIdentityTests
{
    [Fact]
    public void Users_should_inherit_from_IdentityUser()
    {
        Assert.True(typeof(Users).IsSubclassOf(typeof(IdentityUser)));
    }
}
