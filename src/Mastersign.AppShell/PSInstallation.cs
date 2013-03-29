using System;
using System.Reflection;

namespace de.mastersign.shell
{
    public static class PSInstallation
    {
        public static bool IsPowerShellInstalled { get; private set; }

        static PSInstallation()
        {
            try
            {
                var a =
                    Assembly.Load(
                        "System.Management.Automation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                IsPowerShellInstalled = true;
            }
            catch (Exception)
            {
                IsPowerShellInstalled = false;
            }
        }
    }
}