using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace MagicBox.Testing
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    internal class ContextForTests
    {
        // ReSharper disable UnusedMember.Local
        // ReSharper disable ClassNeverInstantiated.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable UnusedParameter.Local
        private class Class_Without_Constructor
        {
        }

        private class Class_With_One_Empty_Constructor
        {
            // ReSharper disable once EmptyConstructor
            public Class_With_One_Empty_Constructor()
            {
            }
        }

        private class Class_With_One_Constructor_Parameter
        {
            public Class_With_One_Constructor_Parameter(IList<string> param1)
            {
                Param1 = param1;
            }

            public IList<string> Param1 { get; }
        }

        private class Class_With_Lazy_Parameter
        {
            public Class_With_Lazy_Parameter(Lazy<IEnumerable<string>> lazyEnumerable, Lazy<IComparable> comparable)
            {
                LazyEnumerable = lazyEnumerable;
                Comparable = comparable;
            }

            public Lazy<IEnumerable<string>> LazyEnumerable { get; }
            public Lazy<IComparable> Comparable { get; }
        }

        private class Class_With_Nested_Constructor_Parameter
        {
            public Class_With_Nested_Constructor_Parameter(Class_With_One_Constructor_Parameter param1)
            {
                Param1 = param1;
            }

            private Class_With_One_Constructor_Parameter Param1 { get; }
        }

        private class Class_With_Two_Constructor_Parameter
        {
            public Class_With_Two_Constructor_Parameter(IList<string> param1, ICloneable cloneable)
            {
            }
        }

        private class Class_With_Optional_Constructor_Parameter
        {
            public Class_With_Optional_Constructor_Parameter(IList<string> param1, ICloneable cloneable = null)
            {
                Cloneable = cloneable;
            }

            public ICloneable Cloneable { get; }
        }

        private class Class_With_Two_Similar_Constructor_Parameter
        {
            public Class_With_Two_Similar_Constructor_Parameter(IList<string> param1, IList<string> param2)
            {
                Param1 = param1;
                Param2 = param2;
            }

            public IList<string> Param1 { get; }
            public IList<string> Param2 { get; }
        }

        private class Class_With_Two_Constructors
        {
            public Class_With_Two_Constructors(IList<string> param1)
            {
                Param1 = param1;
                Param2 = null;
            }

            public Class_With_Two_Constructors(IList<string> param1, IList<string> param2)
            {
                Param1 = param1;
                Param2 = param2;
            }

            public IList<string> Param1 { get; }
            public IList<string> Param2 { get; }
        }

        private class Class_With_Primitive_Constructor_Parameters
        {
            public Class_With_Primitive_Constructor_Parameters(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public string Name { get; }
            public int Age { get; }
        }
        // ReSharper restore once UnusedMember.Local

        [Test]
        public void Cannot_Handle_Class_With_Nested_Constructor_Parameter()
        {
            // because NSubstitute does not support class with constructor parameter
            0.Invoking(x => Substitute.For<Class_With_One_Constructor_Parameter>()).Should().Throw<Exception>();

            // as long as they are public
            1.Invoking(x => Substitute.For<Class_With_One_Empty_Constructor>()).Should().Throw<Exception>();
        }

        [Test]
        public void Handle_Class_Without_Constructor()
        {
            var ctx = new ContextFor<Class_Without_Constructor>();
            var sut = ctx.BuildSut();
            sut.Should().NotBeNull();
        }

        [Test]
        public void Throw_Exception_On_Access_Non_Existing_Parameter()
        {
            var ctx = new ContextFor<Class_With_One_Constructor_Parameter>();

            // on generic
            ctx.Invoking(x => x.For<IFormattable>()).Should().Throw<ArgumentException>();

            // on named parameter
            ctx.Invoking(x => x.For<IList<string>>("this_does_not_exist")).Should().Throw<ArgumentException>();
        }

        [Test]
        public void Throw_Exception_On_Wrong_Expectation_On_Parameter_Type()
        {
            var ctx = new ContextFor<Class_With_One_Constructor_Parameter>();

            // wrong parameter type
            ctx.Invoking(x => x.For<IFormattable>("param1")).Should().Throw<InvalidCastException>();
        }

        [Test]
        public void Handle_Class_With_One_Empty_Constructor()
        {
            var ctx = new ContextFor<Class_With_One_Empty_Constructor>();
            var sut = ctx.BuildSut();
            sut.Should().NotBeNull();
        }

        [Test]
        public void Handle_Class_With_One_Constructor_Parameter()
        {
            var ctx = new ContextFor<Class_With_One_Constructor_Parameter>();
            ctx.For<IList<string>>().Should().NotBeNull();
            ctx.For<IList<string>>().IndexOf(Arg.Any<string>()).ReturnsForAnyArgs(4);

            var sut = ctx.BuildSut();
            sut.Should().NotBeNull();
            sut.Param1.IndexOf("foo").Should().Be(4);
        }

        [Test]
        public void Handle_Class_With_Two_Constructor_Parameter()
        {
            var ctx = new ContextFor<Class_With_Two_Constructor_Parameter>();

            var param1 = ctx.For<IList<string>>();
            var param2 = ctx.For<ICloneable>();
            param1.Should().NotBeNull();
            param2.Should().NotBeNull();

            var sut = ctx.BuildSut();
            sut.Should().NotBeNull();
        }

        [Test]
        public void Handle_Class_With_Optional_Constructor_Parameter_And_No_Substitute_For_Parameter()
        {
            var ctx = new ContextFor<Class_With_Optional_Constructor_Parameter>(false);

            var param1 = ctx.For<IList<string>>();
            var param2 = ctx.For<ICloneable>();
            param1.Should().NotBeNull();
            param2.Should().BeNull();

            var sut = ctx.BuildSut();
            sut.Should().NotBeNull();
        }

        [Test]
        public void Handle_Class_With_Optional_Constructor_Parameter_And_Substitute_For_Parameter()
        {
            var ctx = new ContextFor<Class_With_Optional_Constructor_Parameter>(true);

            var param1 = ctx.For<IList<string>>();
            var param2 = ctx.For<ICloneable>();
            param1.Should().NotBeNull();
            param2.Should().NotBeNull();

            var sut = ctx.BuildSut();
            sut.Should().NotBeNull();
        }

        [Test]
        public void Handle_Class_With_Two_Similar_Constructor_Parameter()
        {
            var ctx = new ContextFor<Class_With_Two_Similar_Constructor_Parameter>();

            // it always should return the first parameter
            var param1 = ctx.For<IList<string>>();
            var param2 = ctx.For<IList<string>>();
            param1.Should().NotBeNull();
            param2.Should().NotBeNull();
            param2.Equals(param1).Should().BeTrue();

            // now we get it explicitly
            var param1a = ctx.For<IList<string>>("param1");
            var param2a = ctx.For<IList<string>>("param2");
            param1a.Should().NotBeNull();
            param2a.Should().NotBeNull();
            param2a.Equals(param1a).Should().BeFalse();

            // lets see if the params got injected properly
            var sut = ctx.BuildSut();
            sut.Should().NotBeNull();

            sut.Param1.Should().Equal(param1a);
            sut.Param2.Should().Equal(param2a);
        }

        [Test]
        public void Handle_Class_With_Two_Constructors()
        {
            var ctx = new ContextFor<Class_With_Two_Constructors>();

            // it always should return the first parameter
            var param1 = ctx.For<IList<string>>();
            var param2 = ctx.For<IList<string>>();
            param1.Should().NotBeNull();
            param2.Should().NotBeNull();
            param2.Equals(param1).Should().BeTrue();

            // now we get it explicitly
            var param1a = ctx.For<IList<string>>("param1");
            var param2a = ctx.For<IList<string>>("param2");
            param1a.Should().NotBeNull();
            param2a.Should().NotBeNull();
            param2a.Equals(param1a).Should().BeFalse();

            // lets see if the params got injected properly
            var sut = ctx.BuildSut();
            sut.Should().NotBeNull();

            sut.Param1.Should().Equal(param1a);
            sut.Param2.Should().Equal(param2a);
        }

        [Test]
        public void Use_Instances()
        {
            var ctx = new ContextFor<Class_With_Optional_Constructor_Parameter>();

            // now lets replace the clonable
            var cloneable = Substitute.For<ICloneable>();
            ctx.Use(cloneable);

            // lets see if the params got injected properly
            var sut = ctx.BuildSut();
            sut.Should().NotBeNull();

            sut.Cloneable.Should().Be(cloneable);

            // get a keynotfound exception if wrong parameter
            ctx.Invoking(c => c.Use("I am not a constructor parameter"))
                .Should().Throw<ArgumentException>();
        }

        [Test(Description = "test also primitives, not only substitutes")]
        public void Support_primitive_instances()
        {
            var ctx = new ContextFor<Class_With_Primitive_Constructor_Parameters>();
            var sut = ctx.BuildSut();
            sut.Should().NotBeNull();

            // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
            sut.Name.Should().Be(string.Empty);
            // ReSharper disable once ArrangeDefaultValueWhenTypeNotEvident
            sut.Age.Should().Be(default(int));

            ctx.Use("Name");
            ctx.Use(42);

            sut = ctx.BuildSut();
            sut.Should().NotBeNull();
            sut.Name.Should().Be("Name");
            sut.Age.Should().Be(42);
        }

        [Test]
        public void Handle_Class_With_Two_Constructors_And_Use_Types()
        {
            var ctx = new ContextFor<Class_With_Two_Constructors>(typeof(IList<string>));

            // it always should return the first parameter
            var param1 = ctx.For<IList<string>>();
            param1.Should().NotBeNull();

            // lets see if the params got injected properly
            var sut = ctx.BuildSut();
            sut.Should().NotBeNull();

            sut.Param1.Should().NotBeNull();
            sut.Param2.Should().BeNull();
        }

        [Test]
        public void Handle_Class_With_Two_Constructors_But_Call_Explicit()
        {
            var info = typeof(Class_With_Two_Constructors).GetConstructor(new[] {typeof(IList<string>)});
            var ctx = new ContextFor<Class_With_Two_Constructors>(info);

            // it always should return the first parameter
            var param1 = ctx.For<IList<string>>();
            var param2 = ctx.For<IList<string>>();
            param1.Should().NotBeNull();
            param2.Should().NotBeNull();
            param2.Equals(param1).Should().BeTrue();

            // now we get it explicitly
            var param1a = ctx.For<IList<string>>("param1");
            ctx.Invoking(c => c.For<IList<string>>("param2")).Should().Throw<ArgumentException>();
            param1a.Should().NotBeNull();

            // lets see if the params got injected properly
            var sut = ctx.BuildSut();
            sut.Should().NotBeNull();

            sut.Param1.Should().Equal(param1a);
            sut.Param2.Should().BeNull();
        }

        [Test]
        public void Support_using_multiple_instances_of_the_same_type()
        {
            var ctx = new ContextFor<Class_With_Two_Similar_Constructor_Parameter>();
            var list1 = new List<string>();
            var list2 = new List<string>();
            ctx.Use<IList<string>>(list2, nameof(Class_With_Two_Similar_Constructor_Parameter.Param2));
            ctx.Use<IList<string>>(list1, nameof(Class_With_Two_Similar_Constructor_Parameter.Param1));
        }

        [Test]
        public void Support_Lazy()
        {
            var ctx = new ContextFor<Class_With_Lazy_Parameter>();

            ctx.Should().NotBeNull();
            ctx.For<Lazy<IEnumerable<string>>>().Value.Should().NotBeNull();
            ctx.For<Lazy<IEnumerable<string>>>().Value.Should().BeAssignableTo<IEnumerable<string>>();

            ctx.For<Lazy<IComparable>>().Value.Should().NotBeNull();
            ctx.For<Lazy<IComparable>>().Value.Should().BeAssignableTo<IComparable>();

            ctx.Lazy<IEnumerable<string>>().Should().BeAssignableTo<IEnumerable<string>>();
            ctx.Lazy<IComparable>().Should().BeAssignableTo<IComparable>();

            ctx.Lazy<IEnumerable<string>>("lazyEnumerable").Should().BeAssignableTo<IEnumerable<string>>();
            ctx.Lazy<IComparable>("comparable").Should().BeAssignableTo<IComparable>();
        }
        // ReSharper restore ClassNeverInstantiated.Local
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore UnusedParameter.Local
    }
}