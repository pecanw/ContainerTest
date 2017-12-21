using System;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace ContainerTest
{
    public class DryIocTest : TestBase
    {
        public DryIocTest(ITestOutputHelper output) : base(output)
        {}

        protected override IServiceProvider InitializeContainer(IServiceCollection services)
        {
            var c = new Container();

            return c.WithDependencyInjectionAdapter(
                descriptors: services,
                throwIfUnresolved: type => true)
                .Resolve<IServiceProvider>();
        }
   }
}