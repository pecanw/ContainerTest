using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace ContainerTest
{
    public class AutofacTest : TestBase
    {
        public AutofacTest(ITestOutputHelper output) : base(output)
        {}

        protected override IServiceProvider InitializeContainer(IServiceCollection services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);
            var container = builder.Build();
            
            return new AutofacServiceProvider(container);
        }
   }
}