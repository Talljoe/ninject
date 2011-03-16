using System.Collections.Generic;

namespace Ninject.Tests.Fakes
{
    public class Armory
    {
        public IEnumerable<IWeapon> Weapons { get; private set; }

        public Armory(IEnumerable<IWeapon> weapons)
        {
            this.Weapons = weapons;
        }
    }
}
