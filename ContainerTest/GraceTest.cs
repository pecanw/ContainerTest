using System;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace ContainerTest
{
    public class GraceTest : TestBase
    {
        public GraceTest(ITestOutputHelper output) : base(output)
        {}

        protected override IServiceProvider InitializeContainer(IServiceCollection services)
        {
            var c = new DependencyInjectionContainer();
            return c.Populate(services);
        }
    }
}