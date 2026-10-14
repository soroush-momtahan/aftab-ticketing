using System.Reflection;

namespace Aftab.App.Application;

public static class AssemblyReference
{
    public static readonly Assembly ApplicationAssembly =  typeof(AssemblyReference).Assembly;
}
