using gpconnect_appointment_checker.Helpers;
using NSubstitute;

namespace EndUserTests;

public class SearchPageHelperTests
{
    [Fact]
    public void BuildSearchTimeCountString_ShowsCorrectTimeInBST()
    {
        // Arrange
        var utcNow = new DateTimeOffset(2025, 06, 15, 10, 0, 0, TimeSpan.Zero); // 10:00 UTC
        var expectedUkTime = new DateTimeOffset(2025, 06, 15, 11, 0, 0, TimeSpan.FromHours(1)); // BST is UTC+1

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(utcNow);

        var timeZone = TimeZoneInfo.CreateCustomTimeZone("Europe/London", TimeSpan.FromHours(1), "BST", "BST");
        var tzProvider = Substitute.For<ITimeZoneProvider>();
        tzProvider.FindSystemTimeZoneById("Europe/London").Returns(timeZone);
        tzProvider.ConvertTime(utcNow, timeZone).Returns(expectedUkTime);

        // Act
        var result = SearchPageHelpers.BuildSearchTimeCountString(3, timeProvider, tzProvider);

        // Assert
        const string expectedString = "As of 15 Jun 2025 11:00 - 3 free slots found";
        Assert.Equal(expectedString, result);
    }

    [Fact]
    public void BuildSearchTimeCountString_ShowsCorrectTimeInGMT()
    {
        // Arrange
        var utcNow = new DateTimeOffset(2025, 01, 15, 10, 0, 0, TimeSpan.Zero); // 10:00 UTC
        var expectedUkTime = new DateTimeOffset(2025, 01, 15, 10, 0, 0, TimeSpan.Zero); // GMT = UTC

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(utcNow);

        var timeZone = TimeZoneInfo.CreateCustomTimeZone("Europe/London", TimeSpan.Zero, "GMT", "GMT");
        var tzProvider = Substitute.For<ITimeZoneProvider>();

        tzProvider.FindSystemTimeZoneById("Europe/London").Returns(timeZone);
        tzProvider.ConvertTime(utcNow, timeZone).Returns(expectedUkTime);

        // Act
        var result = SearchPageHelpers.BuildSearchTimeCountString(3, timeProvider, tzProvider);

        // Assert
        const string expectedString = "As of 15 Jan 2025 10:00 - 3 free slots found";
        Assert.Equal(expectedString, result);
    }

    [Fact]
    public void BuildSearchTimeCountString_ConvertsToSpecifiedTimeZone()
    {
        // Arrange
        var utcNow = new DateTimeOffset(2025, 03, 10, 10, 0, 0, TimeSpan.Zero); // 10:00 UTC
        var expectedTime = new DateTimeOffset(2025, 03, 10, 15, 0, 0, TimeSpan.FromHours(5)); // Simulate +5

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(utcNow);

        var customTimeZone = TimeZoneInfo.CreateCustomTimeZone("Custom/Zone", TimeSpan.FromHours(5), "UTC+5", "UTC+5");
        var tzProvider = Substitute.For<ITimeZoneProvider>();

        tzProvider.FindSystemTimeZoneById("Europe/London").Returns(customTimeZone);
        tzProvider.ConvertTime(utcNow, customTimeZone).Returns(expectedTime);

        // Act
        var result = SearchPageHelpers.BuildSearchTimeCountString(2, timeProvider, tzProvider);

        // Assert
        const string expectedString = "As of 10 Mar 2025 15:00 - 2 free slots found";
        Assert.Equal(expectedString, result);
    }

    [Fact]
    public void BuildSearchTimeCountString_ReturnsNonPluralisedString_WhenSingleResult()
    {
        // Arrange
        var utcNow = new DateTimeOffset(2025, 06, 15, 10, 0, 0, TimeSpan.Zero); // 10:00 UTC
        var expectedUkTime = new DateTimeOffset(2025, 06, 15, 11, 0, 0, TimeSpan.FromHours(1)); // BST is UTC+1

        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(utcNow);

        var timeZone = TimeZoneInfo.CreateCustomTimeZone("Europe/London", TimeSpan.FromHours(1), "BST", "BST");
        var tzProvider = Substitute.For<ITimeZoneProvider>();
        tzProvider.FindSystemTimeZoneById("Europe/London").Returns(timeZone);
        tzProvider.ConvertTime(utcNow, timeZone).Returns(expectedUkTime);

        // Act
        var result = SearchPageHelpers.BuildSearchTimeCountString(1, timeProvider, tzProvider);

        // Assert
        const string expectedString = "As of 15 Jun 2025 11:00 - 1 free slot found";
        Assert.Equal(expectedString, result);
    }
}