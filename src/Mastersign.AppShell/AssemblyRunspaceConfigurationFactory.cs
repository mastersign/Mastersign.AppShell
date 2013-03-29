using System;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace de.mastersign.shell
{
    internal class AssemblyRunspaceConfigurationFactory : IRunspaceConfigurationFactory
    {
        public Assembly[] Assemblies { get; private set; }

        public AssemblyRunspaceConfigurationFactory(params Assembly[] assemblies)
        {
            Assemblies = assemblies;
        }

        public RunspaceConfiguration CreateRunspaceConfiguration()
        {
            var config = RunspaceConfiguration.Create();

            foreach (var assembly in Assemblies)
            {
                var attribs = assembly.GetCustomAttributes(typeof(RunspaceConfigurationTypeAttribute), false);
                if (attribs.Length != 1) continue;

                var configTypeName = attribs[0] as RunspaceConfigurationTypeAttribute;
                if (configTypeName == null) continue;

                var configType = assembly.GetType(configTypeName.RunspaceConfigurationType, false);
                if (configType == null) continue;

                var constructor = configType.GetConstructor(new Type[] { });
                if (constructor == null) continue;

                RunspaceConfiguration assemblyConfig = null;
                try
                {
                    assemblyConfig = constructor.Invoke(new object[] { }) as RunspaceConfiguration;
                }
                catch (TargetInvocationException) { }
                if (assemblyConfig == null) continue;

                config.Assemblies.Append(assemblyConfig.Assemblies);
                config.Cmdlets.Append(assemblyConfig.Cmdlets);
                config.Formats.Append(assemblyConfig.Formats);
                config.InitializationScripts.Append(assemblyConfig.InitializationScripts);
                config.Providers.Append(assemblyConfig.Providers);
                config.Scripts.Append(assemblyConfig.Scripts);
                config.Types.Append(assemblyConfig.Types);
            }

            return config;
        }
    }
}
