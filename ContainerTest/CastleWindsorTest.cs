using System;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace ContainerTest
{
    public class CastleWindsorTest : TestBase
    {
        public CastleWindsorTest(ITestOutputHelper output) : base(output)
        {}

        protected override IServiceProvider InitializeContainer(IServiceCollection services)
        {
            var c = new WindsorContainer();

            c.AddFacility<TypedFactoryFacility>();

            return WindsorRegistrationHelper.CreateServiceProvider(c, services);
        }
   }
}