using Shared.Options.Factory;
using Shared.Options.Implementations;

namespace Utils.Types;

public static class TypeExt
{
    public static Option<Type> GetTypeByName(string typeName)
    {
        var type = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => string.Equals(t.Name, typeName, StringComparison.OrdinalIgnoreCase));
        return type ?? Option<Type>.None();
    }

    public static Option<Type> GetTypeByName(string typeName, string @namespace)
    {
        var type = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(asm => asm.GetTypes())
            .FirstOrDefault(type =>
                string.Equals(type.Name, typeName, StringComparison.OrdinalIgnoreCase) &&
                type.Namespace != null &&
                (string.Equals(type.Namespace, @namespace, StringComparison.OrdinalIgnoreCase) || type.Namespace.StartsWith($"{@namespace}.")));
        return type ?? Option<Type>.None();
    }

    public static Option<Type> GetExactType(string fullyQualifiedName) =>
        Type.GetType(fullyQualifiedName, throwOnError: false, ignoreCase: false) ?? Option<Type>.None();
}