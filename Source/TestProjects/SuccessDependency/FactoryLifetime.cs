namespace SuccessDependency;

using System.Text;

public static class FactoryLifetime
{
    public static readonly StringBuilder Log = new StringBuilder();
    private static int lastAssignedId = 0;

    public static int Created(IIdentifiable @object)
    {
        Log.AppendLine("Created: " + @object.GetType().FullName + " - Id: " + lastAssignedId);
        return lastAssignedId++;
    }

    public static void Initialized(IIdentifiable @object)
    {
        Log.AppendLine("Initialized: " + @object.GetType().FullName + " - Id: " + @object.Id);
    }

    public static void Disposed(IIdentifiable @object)
    {
        Log.AppendLine("Disposed: " + @object.GetType().FullName + " - Id: " + @object.Id);
    }
}