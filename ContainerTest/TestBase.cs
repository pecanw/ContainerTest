using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace ContainerTest
{
    public abstract class TestBase
    {
        protected readonly IServiceProvider _container;
        private readonly ITestOutputHelper _output;

        protected TestBase(ITestOutputHelper output)
        {
            _output = output;

            var services = new ServiceCollection();
            ConfigureServices(services);

            // ReSharper disable once VirtualMemberCallInConstructor
            _container = InitializeContainer(services);
        }

        protected abstract IServiceProvider InitializeContainer(IServiceCollection services);

        protected void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISingletonComponent, SingletonComponentWithScopedFunc>();
            services.AddScoped<IScopedComponent, ScopedComponent>();
        }

        protected IServiceScope BeginScope()
        {
            var serviceScopeFactory = _container.GetRequiredService<IServiceScopeFactory>();
            var serviceScope = serviceScopeFactory.CreateScope();
            return serviceScope;
        }

        [Fact]
        public virtual void Test00_ResolveReturnsSameObjectsInTheSameScope()
        {
            IScopedComponent scoped1, scoped2;

            using (var scope = BeginScope())
            {
                var scopedProvider = scope.ServiceProvider;

                scoped1 = scopedProvider.GetRequiredService<IScopedComponent>();
                scoped2 = scopedProvider.GetRequiredService<IScopedComponent>();
            }

            Assert.Same(scoped2, scoped1);
        }

        [Fact]
        public virtual void Test01_FuncReturnsSameObjectsInTheSameScope()
        {
            ISingletonComponent singleton;
            IScopedComponent scoped1, scoped2;

            using (var scope = BeginScope())
            {
                var scopedProvider = scope.ServiceProvider;

                singleton = scopedProvider.GetRequiredService<ISingletonComponent>();
                scoped1 = singleton.Scoped;
                scoped2 = singleton.Scoped;
            }

            Assert.Same(scoped2, scoped1);
        }

        [Fact]
        public virtual void Test02_FuncAndResolveReturnsSameObjectsInTheSameScope()
        {
            ISingletonComponent singleton;
            IScopedComponent scoped1, scoped2;

            using (var scope = BeginScope())
            {
                var scopedProvider = scope.ServiceProvider;

                singleton = scopedProvider.GetRequiredService<ISingletonComponent>();
                scoped1 = singleton.Scoped;
                scoped2 = scopedProvider.GetRequiredService<IScopedComponent>();
            }

            Assert.Same(scoped2, scoped1);
        }

        [Fact]
        public virtual void Test03_FuncReturnsDifferentObjectsInDifferentScopes()
        {
            ISingletonComponent singleton;
            IScopedComponent scoped1, scoped2;


            using (var scope = BeginScope())
            {
                var scopedProvider = scope.ServiceProvider;

                singleton = scopedProvider.GetRequiredService<ISingletonComponent>();
                scoped1 = singleton.Scoped;
            }

            using (var scope = BeginScope())
            {
                var scopedProvider = scope.ServiceProvider;

                singleton = scopedProvider.GetRequiredService<ISingletonComponent>();
                scoped2 = singleton.Scoped;
            }

            Assert.NotSame(scoped2, scoped1);
        }

        [Fact]
        public virtual void Test04_ResolveReturnsDifferentObjectsInDifferentScopes()
        {
            IScopedComponent scoped1, scoped2;


            using (var scope = BeginScope())
            {
                var scopedProvider = scope.ServiceProvider;

                scoped1 = scopedProvider.GetRequiredService<IScopedComponent>();
            }

            using (var scope = BeginScope())
            {
                var scopedProvider = scope.ServiceProvider;

                scoped2 = scopedProvider.GetRequiredService<IScopedComponent>();
            }

            Assert.NotSame(scoped2, scoped1);
        }

        [Fact]
        public virtual void Test06_MultipleResolvesInTheSameScope()
        {
            ISingletonComponent singleton;
            IScopedComponent scoped1, scoped2, scoped3, scoped4, scoped5, scoped6, scoped7, scoped8;

            using (var scope = BeginScope())
            {
                var scopedProvider = scope.ServiceProvider;

                singleton = scopedProvider.GetRequiredService<ISingletonComponent>();
                var fscoped = scopedProvider.GetRequiredService<Func<IScopedComponent>>();
                var fscoped2 = scopedProvider.GetRequiredService<Func<IScopedComponent>>();

                scoped1 = scopedProvider.GetRequiredService<IScopedComponent>();
                scoped2 = scopedProvider.GetRequiredService<IScopedComponent>();
                scoped3 = fscoped();
                scoped4 = fscoped();
                scoped5 = fscoped2();
                scoped6 = fscoped2();
                scoped7 = singleton.Scoped;
                scoped8 = singleton.Scoped;

                _output.WriteLine("scoped1: {0}", scoped1);
                _output.WriteLine("scoped2: {0}", scoped2);
                _output.WriteLine("scoped3: {0}", scoped3);
                _output.WriteLine("scoped4: {0}", scoped4);
                _output.WriteLine("scoped5: {0}", scoped5);
                _output.WriteLine("scoped6: {0}", scoped6);
                _output.WriteLine("scoped7: {0}", scoped7);
                _output.WriteLine("scoped8: {0}", scoped8);
            }

            Assert.Same(scoped1, scoped2);
            Assert.Same(scoped3, scoped4);
            Assert.Same(scoped5, scoped6);
            Assert.Same(scoped7, scoped8);
            Assert.Same(scoped1, scoped3);
            Assert.Same(scoped1, scoped5);
            Assert.Same(scoped1, scoped7);
        }

    }
}