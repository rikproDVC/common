using Xunit;
using Lisa.Common.Errors;
using System.Collections.Generic;
using System.Resources;

namespace Lisa.Common.UnitTests
{
    public class ErrorManagerTest
    {
        // Default resource tests
        [Fact]
        public void ItReturnsAValidErrorObject()
        {
            ErrorBuilder.Initialize();

            var result = ErrorBuilder.BuildError(11010001, new { field = "Test Field"});

            Assert.Equal("Field Test Field is required.", result.Message);
            Assert.Equal(11010001, result.Code);
        }

        [Fact]
        public void ItCanUseAnIntAsParameter()
        {
            ErrorBuilder.Initialize();

            var result = ErrorBuilder.BuildError(12010003, new { field = "Test Field", value = "Test Value", count = 11 });

            Assert.Equal("The field Test Field with value Test Value doesn't meet the requirements of 11 digits.", result.Message);
            Assert.Equal(12010003, result.Code);
        }

        [Fact]
        public void ItUsesAListAsParameterWithCommaSeparator()
        {
            ErrorBuilder.Initialize();

            var result = ErrorBuilder.BuildError(12010002, new { field = "test", value = "test", values = new string[] { "testing", "test string" } });

            Assert.Equal("The field test with value test can only contain testing, test string.", result.Message);
            Assert.Equal(12010002, result.Code);
        }

        [Fact]
        public void ItReturnsAnEmptyErrorTextWhenTheCodeIsNotFound()
        {
            ErrorBuilder.Initialize();

            var result = ErrorBuilder.BuildError(11000000);

            Assert.Equal(string.Empty, result.Message);
            Assert.Equal(11000000, result.Code);
        }

        [Fact]
        public void ItIgnoresMissingErrorParameters()
        {
            ErrorBuilder.Initialize();

            var result = ErrorBuilder.BuildError(11010001, new { });

            Assert.Equal("Field {field} is required.", result.Message);
            Assert.Equal(11010001, result.Code);
        }

        [Fact]
        public void ItIgnoresExcessErrorParameters()
        {
            ErrorBuilder.Initialize();

            var result = ErrorBuilder.BuildError(11010001, new { field = "Test Field", excess = "excess field" });

            Assert.Equal("Field Test Field is required.", result.Message);
            Assert.Equal(11010001, result.Code);
        }

        // Custom resource tests
        [Fact]
        public void ItCanUseCustomErrorMessages()
        {
            ErrorBuilder.Initialize(ErrorMessages.ResourceManager);

            var result = ErrorBuilder.BuildError(13020002);

            Assert.Equal("Custom static Error works.", result.Message);
            Assert.Equal(13020002, result.Code);
        }

        [Fact]
        public void ItCanUseCustomParameterizedErrorMessages()
        {
            ErrorBuilder.Initialize(ErrorMessages.ResourceManager);

            var result = ErrorBuilder.BuildError(13020001, new { error = "custom" });

            Assert.Equal("Custom Error custom works.", result.Message);
            Assert.Equal(13020001, result.Code);
        }

        [Fact]
        public void ItCanUseMultipleCustomErrorResources()
        {
            ErrorBuilder.Initialize(ErrorMessages.ResourceManager, ErrorMessagesTwo.ResourceManager);

            var result = ErrorBuilder.BuildError(13020002);

            Assert.Equal("Custom static Error works.", result.Message);
            Assert.Equal(13020002, result.Code);

            result = ErrorBuilder.BuildError(19010001);

            Assert.Equal("Second error file works.", result.Message);
            Assert.Equal(19010001, result.Code);
        }
    }
}