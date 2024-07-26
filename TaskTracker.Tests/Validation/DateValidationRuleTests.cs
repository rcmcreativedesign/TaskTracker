using TaskTracker.Validation;

namespace TaskTracker.Tests.Validation
{
    public class DateValidationRuleTests
    {
        [Fact]
        public void EmptyStringIsValid()
        {
            var sut = new DateValidationRule();
            Assert.True(sut.Validate(null, null).IsValid);
        }

        [Fact]
        public void BadStringIsNotValid()
        {
            var sut = new DateValidationRule();
            Assert.False(sut.Validate("13/40/203", null).IsValid);
        }

        [Fact]
        public void GoodDateIsValid()
        {
            var sut = new DateValidationRule();
            Assert.True(sut.Validate(DateTime.Today, null).IsValid);
        }
    }
}
