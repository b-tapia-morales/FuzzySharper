using Kernel.Function.Implementations;
using Reasoning.Proposition.Components;
using Reasoning.Proposition.Implementations;
using Reasoning.Rule.FuzzySet.Components;

namespace Reasoning.Tests.Proposition;

public class PropositionEqualityTests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void BooleanProposition_DeepCopy_IsEqualThroughTypedAndObjectOverloads(
        bool useOr, bool negate)
    {
        var connective = useOr ? Connective.Or : Connective.And;
        var literal = negate ? Literal.IsNot : Literal.Is;
        var proposition = new BooleanProposition<Window>(Window.Open, connective, literal);
        var copy = Assert.IsType<BooleanProposition<Window>>(proposition.DeepCopy());

        Assert.NotSame(proposition, copy);
        Assert.True(proposition.Equals(copy));
        Assert.True(((object)proposition).Equals(copy));
        Assert.True(copy.Equals(proposition));
        Assert.Equal(proposition.GetHashCode(), copy.GetHashCode());
    }

    [Fact]
    public void BooleanProposition_DifferentValueConnectiveOrLiteral_IsNotEqual()
    {
        var proposition = new BooleanProposition<Window>(Window.Open, Connective.And, Literal.Is);

        Assert.False(proposition.Equals(new BooleanProposition<Window>(Window.Closed, Connective.And, Literal.Is)));
        Assert.False(proposition.Equals(new BooleanProposition<Window>(Window.Open, Connective.Or, Literal.Is)));
        Assert.False(proposition.Equals(new BooleanProposition<Window>(Window.Open, Connective.And, Literal.IsNot)));
        Assert.False(proposition.Equals((BooleanProposition<Window>?)null));
    }

    [Fact]
    public void FuzzyProposition_SameTermForDifferentVariables_IsNotEqual()
    {
        var function = TriangleFunction.Create("Cold", 0, 8, 15);
        var room = new FuzzyProposition("Room temperature", Connective.If, Literal.Is, LinguisticHedge.None, function);
        var outside = new FuzzyProposition("Outside temperature", Connective.If, Literal.Is, LinguisticHedge.None, function);
        var roomWithDifferentCasing = new FuzzyProposition("ROOM TEMPERATURE", Connective.If, Literal.Is, LinguisticHedge.None, function);

        Assert.False(room.Equals(outside));
        Assert.False(outside.Equals(room));
        Assert.True(room.Equals(roomWithDifferentCasing));
        Assert.Equal(room.GetHashCode(), roomWithDifferentCasing.GetHashCode());
    }

    [Fact]
    public void FuzzySetConsequent_DeepCopy_IsEqualToConsequentRatherThanProposition()
    {
        var function = TriangleFunction.Create("Low", 0, 20, 40);
        var proposition = new FuzzyProposition("Heating power", Connective.Then, Literal.Is, LinguisticHedge.None, function);
        var consequent = new FuzzySetConsequent(proposition);
        var copy = Assert.IsType<FuzzySetConsequent>(consequent.DeepCopy());

        Assert.NotSame(consequent, copy);
        Assert.True(consequent.Equals(copy));
        Assert.True(copy.Equals(consequent));
        Assert.Equal(consequent.GetHashCode(), copy.GetHashCode());
        Assert.False(consequent.Equals(proposition));
        Assert.False(consequent.Equals(null));
    }
}
