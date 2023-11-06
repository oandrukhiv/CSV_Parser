using Xunit;

namespace CSV_Parser.Tests
{
    public class Tests
    {
        readonly string testString = "123";

        [Fact]
        public void TestConverter()
        {
            var convertedToInt =  ProcessCSVService.Converter(testString) ;
            Assert.NotNull(convertedToInt);
            Assert.IsType<int>(convertedToInt);
        }

    }
}
