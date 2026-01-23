using LogParser.Api.Services;

namespace LogParser.Tests
{
    public class LogParserServiceTests
    {
        private readonly ILogParserService service = new LogParserService();

        [Fact]
        public void Analyze_EmptyInput_ReturnsEmptyResult()
        {
            var result = service.Analyze(Array.Empty<string>());

            Assert.Equal(0, result.UniqueIpCount);
            Assert.Empty(result.TopUrls);
            Assert.Empty(result.TopIpAddresses);
        }

        [Fact]
        public void Analyze_SingleLogLine_ParsesCorrectly()
        {
            var lines = new[]
            {
                @"177.71.128.21 - - [10/Jul/2018:22:21:28 +0200] ""GET /home HTTP/1.1"" 200 123"
            };

            var result = service.Analyze(lines);

            Assert.Equal(1, result.UniqueIpCount);
            Assert.Equal("/home", result.TopUrls.Single());
            Assert.Equal("177.71.128.21", result.TopIpAddresses.Single());
        }

        [Fact]
        public void Analyze_MultipleLines_ReturnsTopThree()
        {
            var lines = new[]
            {
                @"1.1.1.1 - - ""GET /a HTTP/1.1"" 200 1",
                @"1.1.1.1 - - ""GET /a HTTP/1.1"" 200 1",
                @"2.2.2.2 - - ""GET /b HTTP/1.1"" 200 1",
                @"3.3.3.3 - - ""GET /c HTTP/1.1"" 200 1"
            };

            var result = service.Analyze(lines);

            Assert.Equal(3, result.UniqueIpCount);
            Assert.Equal("/a", result.TopUrls.First());
            Assert.Equal("1.1.1.1", result.TopIpAddresses.First());
        }

        [Fact]
        public void ParseLine_WithTrailingJunk_StillParsesIpAndUrl()
        {
            var lines = new[]
            {
                @"72.44.32.10 - - [09/Jul/2018:15:48:07 +0200] ""GET / HTTP/1.1"" 200 3574 ""-"" ""UA"" junk extra"
            };

            var result = service.Analyze(lines);

            Assert.NotNull(result);
            Assert.Equal(1, result.UniqueIpCount);
            Assert.Equal("72.44.32.10", result.TopIpAddresses.Single());
            Assert.Equal("/", result.TopUrls.Single());
        }

        [Fact]
        public void ParseLine_CompletelyInvalid_ReturnsNull()
        {
            var lines = new[] { "this is not a log line" };

            var result = service.Analyze(lines);

            Assert.Equal(0, result.UniqueIpCount);
            Assert.Empty(result.TopUrls);
            Assert.Empty(result.TopIpAddresses);
        }
    }
}