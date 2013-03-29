using System.Management.Automation.Runspaces;

namespace de.mastersign.shell
{
    public interface IRunspaceConfigurationFactory
    {
        RunspaceConfiguration CreateRunspaceConfiguration();
    }
}
