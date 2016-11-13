using FluentAssertions;
using RequestIdValidator;
using Xunit;

namespace UnitTests
{
    public class Tests
    {
        [Fact]
        public void IntEquals()
        {
            var obj = new SimpleIntObject();
            obj.Id.ShouldBeEquivalentTo(1);
            IdentityValidator.VerifyIds(obj.Id, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
            obj.Id.ShouldBeEquivalentTo(1);

            obj.Id = 0;
            IdentityValidator.VerifyIds(1, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
            obj.Id.ShouldBeEquivalentTo(1);

            obj.Id = 1;
            IdentityValidator.VerifyIds(0, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.ErrorUnequalIds);

            obj.Id = 1;
            IdentityValidator.VerifyIds(obj.Id, obj, null).Status.ShouldBeEquivalentTo(ValidationResult.ErrorMissingOrInvalidLamda);
            IdentityValidator.VerifyIds<SimpleIntObject>(1, null, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.ErrorMissingBody);
        }

        [Fact]
        public void StringEquals()
        {
            var obj = new SimpleStringObject();
            IdentityValidator.VerifyIds(obj.Id, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
        }

        [Fact]
        public void GuidEquals()
        {
            var obj = new SimpleGuidObject();
            IdentityValidator.VerifyIds(obj.Id, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
        }

        [Fact]
        public void ActualTest()
        {
            var obj = new SimpleIntObject();
            IIdentityValidator validator = new IdentityValidator();

            Assert.Throws<IdentityException>(() => {
                validator.VerifyIds(3, obj, o => o.Id).ThrowExceptionOnError();
            });
        }
    }
}