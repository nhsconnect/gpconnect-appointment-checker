using Xunit;

namespace gpconnect_appointment_checker.UnitTest
{
    public class ExampleTest
    {
        [Fact]
        public void ComparisonEquality()
        {
            var expected = 10;
            int actual = 10;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ComparisonInequality()
        {
            var expected = 10;
            int actual = 20;
            Assert.NotEqual(expected, actual);
        }
    }
}
