using System.Reflection;

namespace ETor.Shared;

public class AssemblyUtils
{
    public static IEnumerable<Type> GetAssignableTypesFromAssembly<T>(Assembly assembly)
    {
        return assembly.ExportedTypes
            .Where(t => t.IsAssignableTo(typeof(T)) && t is {IsAbstract: false, IsInterface: false});
    }

    public static IList<Type> GetAssignableTypes<T>()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(GetAssignableTypesFromAssembly<T>)
            .ToList();

        return types;
    }
}