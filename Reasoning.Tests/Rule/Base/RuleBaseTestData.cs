using Shared.Primitives.Implementation;

namespace Reasoning.Tests.Rule.Base;

public sealed class BaseVariables : TheoryData<StringOrType>
{
    public BaseVariables()
    {
        Add("Room temperature");
        Add("Humidity");
        Add("Outside temperature");
        Add(typeof(Window));
        Add(typeof(Occupancy));
    }
}
