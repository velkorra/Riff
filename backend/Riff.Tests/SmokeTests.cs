namespace Riff.Tests;

public class SmokeTests
{
    [Fact]
    public void Math_Should_Work()
    {
        Assert.Equal(4, 2 + 2);
    }

    [Fact]
    public void True_Is_True()
    {
        Assert.True(true);
    }

    [Fact]
    public void Dto_Mapping_ShouldNotCrash()
    {
        var room = new Riff.Api.Contracts.Dto.CreateRoomRequest("Test Room", "123");
        Assert.NotNull(room);
        Assert.Equal("Test Room", room.Name);
    }
}