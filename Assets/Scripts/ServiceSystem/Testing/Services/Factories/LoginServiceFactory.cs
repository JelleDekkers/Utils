using Utils.Core.Services;

public class LoginServiceFactory : IServiceFactory<LoginService>
{
    public bool testUseMockData = false;

    public LoginService Construct()
    {
        ITestServer server;
        if(testUseMockData)
        {
            server = new MockTestServer();
        }
        else
        {
            server = new RealTestServer();
        }

        return new LoginService(server);
    }
}
