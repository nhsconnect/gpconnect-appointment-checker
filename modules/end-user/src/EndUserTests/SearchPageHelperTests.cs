using Xunit;
using NSubstitute; // NSubstitute namespace
using System;
using Microsoft.Extensions.Time.Testing; // For FakeTimeProvider
using FluentAssertions;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using gpconnect_appointment_checker.Helpers.Extensions;

// Make sure ITimeZoneProvider is in a namespace accessible here
// using gpconnect_appointment_checker.Helpers.Time; // Example namespace

public class SearchPageHelperTests
{
    [Theory]
    [InlineData(2025, 5, 29, 14, 30, "Europe/London", "29 May 15:30", true, "BST (UTC+1) period")] // BST
    [InlineData(2025, 1, 15, 14, 30, "Europe/London", "15 Jan 14:30", false, "GMT (UTC+0) period")] // GMT
    [InlineData(2025, 5, 29, 14, 30, "America/New_York", "29 May 10:30", false, "Another timezone (EDT, UTC-4)")]
    public void BuildSearchTimeCountString_HandlesTimeZonesCorrectly_WithNSubstitute(
        int year, int month, int day, int hour, int minute,
        string timeZoneId, string expectedTimePart, bool expectIsDst, string testDescription)
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var subTimeZoneProvider = Substitute.For<ITimeZoneProvider>();

        var utcInputTime = new DateTimeOffset(year, month, day, hour, minute, 0, TimeSpan.Zero);
        fakeTimeProvider.SetUtcNow(utcInputTime);

        var realTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        subTimeZoneProvider.FindSystemTimeZoneById(timeZoneId).Returns(realTimeZoneInfo);

        subTimeZoneProvider
            .ConvertTime(Arg.Any<DateTimeOffset>(), Arg.Any<TimeZoneInfo>())
            .ReturnsForAnyArgs(ci =>
            {
                // Get the actual DateTimeOffset passed to ConvertTime
                var inputDto = ci.Arg<DateTimeOffset>();
                // Get the actual TimeZoneInfo passed to ConvertTime
                var destTz = ci.Arg<TimeZoneInfo>();

                // Simulate the conversion based on the expected behavior for Europe/London
                if (destTz.Id == "Europe/London")
                {
                    if (expectIsDst) // If it should be BST (UTC+1)
                        return inputDto.ToOffset(TimeSpan.FromHours(1));
                    else // If it should be GMT (UTC+0)
                        return inputDto.ToOffset(TimeSpan.FromHours(0));
                }

                // For other time zones, you'd implement their specific conversion logic here
                // For "America/New_York", EDT is UTC-4.
                if (destTz.Id == "America/New_York")
                {
                    // For simplicity, hardcode the offset for the test date.
                    // In a real scenario, you might have a more sophisticated fake for DST.
                    return inputDto.ToOffset(TimeSpan.FromHours(-4)); // EDT is UTC-4
                }

                throw new InvalidOperationException($"Unexpected timezone ID for mock: {destTz.Id}");
            });

        const int count = 2; // Fixed count for simplicity in these tests

        // Act
        // Pass the NSubstitute substitute for ITimeZoneProvider
        var result = SearchPageHelpers.BuildSearchTimeCountString(count, fakeTimeProvider, subTimeZoneProvider);

        // Assert
        var expectedSearchAtDate = string.Format(SearchConstants.SearchAtDate, expectedTimePart);
        var expectedCountString = SearchConstants.SearchStatsCountText.Pluraliser(count);
        var expected = $"{expectedSearchAtDate} - {expectedCountString}";

        result.Should().Be(expected, $"because {testDescription}");

        // Optional: Verify that the methods were called as expected
        subTimeZoneProvider.Received(1).FindSystemTimeZoneById("Europe/London");
        subTimeZoneProvider.Received(1).ConvertTime(utcInputTime, realTimeZoneInfo);
    }

    [Fact]
    public void BuildSearchTimeCountString_PluraliserWorksForSingleCount_WithNSubstitute()
    {
        // Arrange
        var fakeTimeProvider = new FakeTimeProvider();
        var subTimeZoneProvider = Substitute.For<ITimeZoneProvider>();

        var utcInputTime = new DateTimeOffset(2025, 5, 29, 14, 30, 0, TimeSpan.Zero); // BST period
        fakeTimeProvider.SetUtcNow(utcInputTime);

        // Mock FindSystemTimeZoneById
        var realLondonTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
        subTimeZoneProvider.FindSystemTimeZoneById("Europe/London").Returns(realLondonTimeZone);

        // Mock ConvertTime to simulate BST conversion
        subTimeZoneProvider
            .ConvertTime(Arg.Any<DateTimeOffset>(), Arg.Any<TimeZoneInfo>())
            .ReturnsForAnyArgs(utcInputTime.ToOffset(TimeSpan.FromHours(1))); // Simulate BST (UTC+1)

        const int count = 1;

        // Act
        var result = SearchPageHelpers.BuildSearchTimeCountString(count, fakeTimeProvider, subTimeZoneProvider);

        // Assert
        var expectedTimePart = "29 May 15:30"; // 14:30 UTC + 1 hour BST
        var expectedSearchAtDate = string.Format(SearchConstants.SearchAtDate, expectedTimePart);
        var expectedCountString = SearchConstants.SearchStatsCountText.Pluraliser(count); // Should be "1 free slot"
        var expected = $"{expectedSearchAtDate} - {expectedCountString}";

        result.Should().Be(expected);

        // Optional: Verify calls
        subTimeZoneProvider.Received(1).FindSystemTimeZoneById("Europe/London");
        subTimeZoneProvider.Received(1).ConvertTime(utcInputTime, realLondonTimeZone);
    }
}