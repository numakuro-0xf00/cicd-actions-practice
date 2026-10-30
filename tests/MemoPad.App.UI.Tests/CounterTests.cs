using Bunit;
using MemoPad.App.UI.Pages;

namespace MemoPad.App.UI.Tests;

public class CounterTests : BunitContext
{
    [Fact]
    public void ClickingButton_IncrementsCount()
    {
        var cut = Render<Counter>();

        cut.Find("button").Click();

        cut.Find("p[role=status]").MarkupMatches("<p role=\"status\">Current count: 1</p>");
    }
}
