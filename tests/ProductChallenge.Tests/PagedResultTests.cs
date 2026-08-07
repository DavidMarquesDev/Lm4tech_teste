using ProductChallenge.Application;

namespace ProductChallenge.Tests;

public class PagedResultTests
{
    private static PagedResult<string> Page(int totalCount, int pageNumber, int pageSize) =>
        new([], totalCount, pageNumber, pageSize);

    [Theory]
    [InlineData(100, 15, 7)]
    [InlineData(100, 10, 10)]
    [InlineData(100, 100, 1)]
    [InlineData(101, 100, 2)]
    [InlineData(1, 10, 1)]
    public void PageCount_RoundsUp(int totalCount, int pageSize, int expected)
    {
        Assert.Equal(expected, Page(totalCount, 1, pageSize).PageCount);
    }

    [Fact]
    public void PageCount_WithoutResults_IsOneSoTheScreenStillShowsAPage()
    {
        Assert.Equal(1, Page(totalCount: 0, pageNumber: 1, pageSize: 10).PageCount);
    }
}
