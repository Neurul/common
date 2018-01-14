using Moq;
using Nancy;
using Nancy.Bootstrapper;
using org.neurul.Common.Http.Cli;
using System;
using Xunit;

namespace org.neurul.Common.Http.Test.Cli.MultiHostProgramFixture.given
{
    public abstract class Context
    {
        public virtual string AppName => "AppName";
        public virtual string[] UriStrings => new string[0];
        public virtual string[] UriNames => new string[0];
        public virtual INancyBootstrapper[] Bootstrappers => new INancyBootstrapper[0];

        public void Invoke()
        {
            var console = new Mock<IConsoleWrapper>();

            console
                .Setup<ConsoleKeyInfo>(e => e.ReadKey(It.IsAny<bool>()))
                .Returns(new ConsoleKeyInfo('Y', ConsoleKey.Y, false, false, false));

            MultiHostProgram.Start(
                console.Object,
                this.AppName,
                this.UriStrings,
                this.UriNames,
                this.Bootstrappers
                );
        }
    }

    public class When_starting
    {
        public class When_specifying_AppName
        {
            public class When_null : Context
            {
                public override string AppName => null;

                [Fact]
                public void Should_throw_argument_null_exception()
                {
                    Assert.Throws<ArgumentNullException>(() => this.Invoke());
                }
            }

            public class When_empty : Context
            {
                public override string AppName => string.Empty;

                [Fact]
                public void Should_throw_argument_exception()
                {
                    Assert.Throws<ArgumentException>(() => this.Invoke());
                }
            }
        }

        public class When_specifying_UriStrings
        {
            public class When_null : Context
            {
                public override string[] UriStrings => null;

                [Fact]
                public void Should_throw_argument_null_exception()
                {
                    Assert.Throws<ArgumentNullException>(() => this.Invoke());
                }
            }

            public class When_two_required
            {
                public class When_none_provided : Context
                {
                    public override INancyBootstrapper[] Bootstrappers => new INancyBootstrapper[]
                    {
                        new DefaultNancyBootstrapper(),
                        new DefaultNancyBootstrapper()
                    };

                    [Fact]
                    public void Should_throw_argument_exception()
                    {
                        Assert.Throws<ArgumentException>(() => this.Invoke());
                    }

                    [Fact]
                    public void Should_throw_argument_exception_mentioning_number_of_missing_strings()
                    {
                        var a = Assert.Throws<ArgumentException>(() => this.Invoke());
                        Assert.Contains("2", a.Message);
                    }

                    [Fact]
                    public void Should_throw_argument_exception_mentioning_name_of_deficient_argument()
                    {
                        var a = Assert.Throws<ArgumentException>(() => this.Invoke());
                        Assert.Contains("URI values", a.Message);
                    }
                }

                public class When_two_provided
                {
                    public class When_both_invalid : Context
                    {
                        private const string Name1 = "name1";

                        public override INancyBootstrapper[] Bootstrappers => new INancyBootstrapper[]
                        {
                            new DefaultNancyBootstrapper(),
                            new DefaultNancyBootstrapper()
                        };

                        public override string[] UriNames => new string[] { Name1, "name2" };
                        
                        public override string[] UriStrings => new string[] { "1", "1" };

                        [Fact]
                        public void Should_throw_argument_exception()
                        {
                            Assert.Throws<ArgumentException>(() => this.Invoke());
                        }

                        [Fact]
                        public void Should_throw_argument_exception_mentioning_reason()
                        {
                            var a = Assert.Throws<ArgumentException>(() => this.Invoke());
                            Assert.Contains($"Must specify valid '{Name1}' URI", a.Message);
                        }
                    }

                    public class When_second_invalid : Context
                    {
                        private const string Name1 = "name1";
                        private const string Name2 = "name2";

                        public override INancyBootstrapper[] Bootstrappers => new INancyBootstrapper[]
                        {
                            new DefaultNancyBootstrapper(),
                            new DefaultNancyBootstrapper()
                        };

                        public override string[] UriNames => new string[] { Name1, Name2 };

                        public override string[] UriStrings => new string[] { "http://localhost:5000", "1" };

                        [Fact]
                        public void Should_throw_argument_exception()
                        {
                            Assert.Throws<ArgumentException>(() => this.Invoke());
                        }

                        [Fact]
                        public void Should_throw_argument_exception_mentioning_reason()
                        {
                            var a = Assert.Throws<ArgumentException>(() => this.Invoke());
                            Assert.Contains($"Must specify valid '{Name2}' URI", a.Message);
                        }
                    }
                }
            }
        }

        public class When_specifying_UriNames
        {
            public class When_null : Context
            {
                public override string[] UriNames => null;

                [Fact]
                public void Should_throw_argument_null_exception()
                {
                    Assert.Throws<ArgumentNullException>(() => this.Invoke());
                }
            }

            public class When_two_required : Context
            {
                public override INancyBootstrapper[] Bootstrappers => new INancyBootstrapper[]
                {
                    new DefaultNancyBootstrapper(),
                    new DefaultNancyBootstrapper()
                };

                public override string[] UriStrings => new string[] { "1", "1" };

                [Fact]
                public void Should_throw_argument_exception()
                {
                    Assert.Throws<ArgumentException>(() => this.Invoke());
                }

                [Fact]
                public void Should_throw_argument_exception_mentioning_number_of_missing_names()
                {
                    var a = Assert.Throws<ArgumentException>(() => this.Invoke());
                    Assert.Contains("2", a.Message);
                }

                [Fact]
                public void Should_throw_argument_exception_mentioning_name_of_deficient_argument()
                {
                    var a = Assert.Throws<ArgumentException>(() => this.Invoke());
                    Assert.Contains("URI names", a.Message);
                }
            }
        }
    }
}
