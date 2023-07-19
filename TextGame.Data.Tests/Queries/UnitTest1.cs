using TextGame.Data.Queries;

namespace TextGame.Data.Tests.Queries;

public class GameContextSourceTests
{
    [Fact]
    public void Test1()
    {
        var subject = new GameContextSource();

        subject.Get(1, "");
    }
}
