using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracker.Validation;

namespace TaskTracker.Tests.Validation
{
    public class RequiredValidationRuleTests
    {
        [Fact]
        public void EmptyStringIsNotValid()
        {
            var sut = new RequiredValidationRule();
            Assert.False(sut.Validate(null, null).IsValid);
        }

        [Fact]
        public void GoodStringIsValid()
        {
            var sut = new RequiredValidationRule();
            Assert.True(sut.Validate("TEST", null).IsValid);
        }
    }
}
