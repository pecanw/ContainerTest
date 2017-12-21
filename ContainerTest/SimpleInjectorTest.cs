using System;
using System.Linq;
using System.Linq.Expressions;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;
using Xunit.Abstractions;

namespace ContainerTest
{
    public class SimpleInjectorSeriveScope : IServiceScope
    {
        private readonly Scope _scope;

        public SimpleInjectorSeriveScope(Scope scope)
        {
            _scope = scope;
            ServiceProvider = scope.Container;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public IServiceProvider ServiceProvider { get; }
    }

    public class SimpleInjectorSeriveScopeFactory : IServiceScopeFactory
    {
        private readonly Container _container;

        public SimpleInjectorSeriveScopeFactory(Container container)
        {
            _container = container;
        }

        public IServiceScope CreateScope()
        {
            return new SimpleInjectorSeriveScope(AsyncScopedLifestyle.BeginScope(_container));
        }
    }

    public static class SimpleInjectorExtensions
    {
        public static void AddFactoryResolver(this Container c)
        {
            c.Options.Container.ResolveUnregisteredType += (s, e) =>
            {
                var type = e.UnregisteredServiceType;

                if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(Func<>))
                {
                    return;
                }

                var serviceType = type.GetGenericArguments().First();

                var registration = c.Options.Container.GetRegistration(serviceType, true);

                var funcType = typeof(Func<>).MakeGenericType(serviceType);

                var factoryDelegate = Expression.Lambda(funcType, registration.BuildExpression()).Compile();

                e.Register(Expression.Constant(factoryDelegate));
            };
        }
    }

    public class SimpleInjectorTest : TestBase
    {
        public SimpleInjectorTest(ITestOutputHelper output) : base(output)
        {}

        protected override IServiceProvider InitializeContainer(IServiceCollection services)
        {
            var c = new Container();

            c.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            c.AddFactoryResolver();

            c.Register<ISingletonComponent, SingletonComponentWithScopedFunc>(Lifestyle.Singleton);
            c.Register<IScopedComponent, ScopedComponent>(Lifestyle.Scoped);
            c.Register<IServiceScopeFactory, SimpleInjectorSeriveScopeFactory>();

            return c;
        }
   }
}