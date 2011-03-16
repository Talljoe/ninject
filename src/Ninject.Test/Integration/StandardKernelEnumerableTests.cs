namespace Ninject.Tests.Integration
{
    using System.Collections.Generic;
    using Ninject.Tests.Fakes;
    using Ninject.Tests.Integration.StandardKernelTests;
    using Ninject.Tests.MSTestAttributes;
    using Xunit;
    using Xunit.Should;

    [TestClass]
    public class WhenAnExplicitIEnumerableIsRegistered : StandardKernelContext
    {
        public WhenAnExplicitIEnumerableIsRegistered()
        {
            this.kernel.Bind<IEnumerable<IWeapon>>().ToMethod(c => new[] { new Shuriken(), });
        }

        [Fact]
        public void GetShouldReturnExplicitEnumerable()
        {
            var service = this.kernel.Get<IEnumerable<IWeapon>>();
            service.ShouldNotBeNull();
            service.ShouldNotBeEmpty();
        }

        [Fact]
        public void GetAllShouldReturnEmpty()
        {
            var service = this.kernel.GetAll<IWeapon>();
            service.ShouldNotBeNull();
            service.ShouldBeEmpty();
        }

        [Fact]
        public void InjectionShouldInjectExplicitEnumerable()
        {
            var service = this.kernel.Get<Armory>();
            service.Weapons.ShouldNotBeNull();
            service.Weapons.ShouldNotBeEmpty();
        }
    }

    [TestClass]
    public class WhenNoEnumerableIsRegistered : StandardKernelContext
    {
        public WhenNoEnumerableIsRegistered()
        {
            this.kernel.Bind<IWeapon>().ToConstant(new Shuriken());
        }

        [Fact]
        public void GetShouldReturnImplicitEnumerable()
        {
            var service = this.kernel.Get<IEnumerable<IWeapon>>();
            service.ShouldNotBeNull();
            service.ShouldNotBeEmpty();
        }

        [Fact]
        public void GetAllShouldReturnImplicitEnumerable()
        {
            var service = this.kernel.GetAll<IWeapon>();
            service.ShouldNotBeNull();
            service.ShouldNotBeEmpty();
        }

        [Fact]
        public void InjectionShouldInjectImplicitEnumerable()
        {
            var service = this.kernel.Get<Armory>();
            service.Weapons.ShouldNotBeNull();
            service.Weapons.ShouldNotBeEmpty();
        }
    }
}