using FluentAssertions;
using RequestIdValidator;
using System;
using Xunit;

namespace UnitTests
{
    public class Tests
    {
        [Fact]
        public void DeepDeepNested_Copy()
        {
            var obj = new DeepDeepNestedObject { Child = new DeepNestedObject { Child = new NestedObject { Child = new SimpleIntObject { Id = 0 } } } };

            IdentityValidator.VerifyIds(9, obj, o => o.Child.Child.Child.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
            obj.Child.Child.Child.Id.ShouldBeEquivalentTo(9);
        }

        [Fact]
        public void DeepDeepNested_Copy2()
        {
            var obj = new DeepDeepNestedObject { Child = new DeepNestedObject { Child = new NestedObject { Child = new SimpleIntObject { Id = 0 } } } };

            IdentityValidator.VerifyIds(9, obj.Child?.Child?.Child, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
            obj.Child?.Child?.Child?.Id.ShouldBeEquivalentTo(9);
        }

        [Fact]
        public void DeepDeepNested_Copy3()
        {
            var obj = new DeepDeepNestedObject { Child = new DeepNestedObject { Child = new NestedObject { Child = new SimpleIntObject { Id = 0 } } } };

            IdentityValidator.VerifyIds(9, obj.Child?.Child?.Child, o => obj.Child.Child.Child.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
            obj.Child?.Child?.Child?.Id.ShouldBeEquivalentTo(9);
        }

        [Fact]
        public void DeepNested_Copy()
        {
            var obj = new DeepNestedObject { Child = new NestedObject { Child = new SimpleIntObject { Id = 0 } } };

            IdentityValidator.VerifyIds(9, obj, o => o.Child.Child.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
            obj.Child.Child.Id.ShouldBeEquivalentTo(9);
        }

        [Fact]
        public void DeepNested_Equal()
        {
            var obj = new DeepNestedObject();
            IdentityValidator.VerifyIds(obj.Child.Child.Id, obj, o => o.Child.Child.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
        }

        [Fact]
        public void DeepNested_NotEqual()
        {
            var obj = new DeepNestedObject();
            IdentityValidator.VerifyIds(9, obj, o => o.Child.Child.Id).Status.ShouldBeEquivalentTo(ValidationResult.ErrorNotEqualIds);
        }

        [Fact]
        public void Guid_Copy()
        {
            var newGuid = Guid.NewGuid();
            var obj = new SimpleGuidObject { Id = Guid.Empty };
            IdentityValidator.VerifyIds(newGuid, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
            obj.Id.ShouldBeEquivalentTo(newGuid);
        }

        [Fact]
        public void Guid_Equals()
        {
            var obj = new SimpleGuidObject();
            IdentityValidator.VerifyIds(obj.Id, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
        }

        [Fact]
        public void Guid_NotEqual()
        {
            var guidInUrl = Guid.NewGuid();
            var guidInBody = Guid.NewGuid();

            var obj = new SimpleGuidObject { Id = guidInBody };
            IdentityValidator.VerifyIds(guidInUrl, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.ErrorNotEqualIds);

            //Debatable: If error occur what id should be used?
            obj.Id.ShouldBeEquivalentTo(guidInBody);
        }

        [Fact]
        public void HandleWierdTypes_NotEqual()
        {
            var obj = new SimpleGuidObject { Id = Guid.NewGuid() };
            IdentityValidator.VerifyIds(1, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.ErrorNotEqualIds);
        }

        [Fact]
        public void Int_Copy()
        {
            var obj = new SimpleIntObject { Id = 0 };
            IdentityValidator.VerifyIds(1, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
            obj.Id.ShouldBeEquivalentTo(1);
        }

        [Fact]
        public void Int_Equals()
        {
            var obj = new SimpleIntObject();
            obj.Id.ShouldBeEquivalentTo(1);
            IdentityValidator.VerifyIds(obj.Id, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
            obj.Id.ShouldBeEquivalentTo(1);
        }

        [Fact]
        public void Int2_Equals()
        {
            var obj = new SimpleIntObject();
            obj.Id = 0;
            IdentityValidator.VerifyIds(1, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
            obj.Id.ShouldBeEquivalentTo(1);
        }

        [Fact]
        public void Int_NotEqual()
        {
            var obj = new SimpleIntObject { Id = 1 };
            IdentityValidator.VerifyIds(5, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.ErrorNotEqualIds);

            //Debatable: If error occur what id should be used?
            obj.Id.ShouldBeEquivalentTo(1);
        }

        [Fact]
        public void Int_OtherErrors()
        {
            var obj = new SimpleIntObject { Id = 1 };
            IdentityValidator.VerifyIds(obj.Id, obj, o => null).Status.ShouldBeEquivalentTo(ValidationResult.ErrorMissingOrInvalidLamda);
            IdentityValidator.VerifyIds<SimpleIntObject>(1, null, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.ErrorMissingBody);
        }

        [Fact]
        public void Nested_Equal()
        {
            var obj = new NestedObject();
            IdentityValidator.VerifyIds(obj.Child.Id, obj, o => o.Child.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
        }

        [Fact]
        public void RealityTest()
        {
            var obj = new SimpleIntObject();
            IIdentityValidator validator = new IdentityValidator();

            Assert.Throws<IdentityException>(() => { validator.VerifyIds(3, obj, o => o.Id).ThrowExceptionOnError(); });
        }

        [Fact]
        public void String_Equals()
        {
            var obj = new SimpleStringObject();
            IdentityValidator.VerifyIds(obj.Id, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);
        }

        [Fact]
        public void String2_Equals()
        {
            var obj = new SimpleStringObject();
            obj.Id = null;
            IdentityValidator.VerifyIds("Test", obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.Valid);

            obj.Id.ShouldBeEquivalentTo("Test");
        }

        [Fact]
        public void AllNull_Equals()
        {
            var obj = new SimpleStringObject();
            obj.Id = null;
            IdentityValidator.VerifyIds(null, obj, o => o.Id).Status.ShouldBeEquivalentTo(ValidationResult.ErrorMissingIds);
        }
    }
}