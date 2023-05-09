using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RedArrisApi.IntegrationTests;

[TestClass]
public static class TestManager
{
    private static IServiceScope _scope;
    private static TestWebFactory<Program> _webFactory;

    public static T GetRequiredService<T>() where T : class
    {
        return _scope.ServiceProvider.GetRequiredService<T>();
    }

    public static TestWebFactory<Program> GetWebApplication()
    {
        return _webFactory;
    }

    [AssemblyInitialize]
    public static void Initialize(TestContext _)
    {
        _webFactory = new TestWebFactory<Program>();
        _scope = _webFactory.Services.CreateScope();
    }

    [AssemblyCleanup]
    public static void CleanUp()
    {
        _scope.Dispose();
        _webFactory.Dispose();
    }
}