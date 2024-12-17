using Xunit.Abstractions;
using Xunit.Sdk;

namespace Crypto_Portfolio_API_IntegrationTests.Helper;

public class PriorityOrderer: ITestCaseOrderer
{
    //implement this member
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
    {
       return testCases.OrderBy(tc => tc.TestMethod.Method.GetCustomAttributes(typeof(TestPriorityAttribute))
           .FirstOrDefault()?.GetNamedArgument<int>("Priority")?? 0);
    }
}

public class TestPriorityAttribute(int priority) : Attribute
{
    public int Priority { get; } = priority;
}