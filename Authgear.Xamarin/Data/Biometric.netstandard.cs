using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin.Data
{
    internal class Biometric : IBiometric
    {
        private readonly IContainerStorage containerStorage;
        internal Biometric(IContainerStorage containerStorage)
        {
            this.containerStorage = containerStorage;
        }
        public void RemoveBiometric(string name)
        {
            throw new NotImplementedException();
        }
    }
}
