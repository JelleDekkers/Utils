using UnityEngine;

public interface ITestServer
{
    void Login();
}

public class MockTestServer : ITestServer
{
    public MockTestServer()
    {
    }

    public void Login()
    {
    }
}

public class RealTestServer : ITestServer
{
    public RealTestServer()
    {
    }

    public void Login()
    {
    }
}
