using System.Collections.Generic;
using Cortside.Common.BootStrap;
using Cortside.RestFS.BootStrap.Installer;

namespace Cortside.RestFS.BootStrap {
    public class DefaultApplicationBootStrapper : BootStrapper {
        public DefaultApplicationBootStrapper() {
            installers = new List<IInstaller> {
                new RepositoryInstaller(),
                new DomainServiceInstaller(),
                new FacadeInstaller()
            };
        }
    }
}
