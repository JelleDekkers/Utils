using Utils.ServiceSystem;

public class LoginService : IService
{
    public bool IsLoggedIn { get; private set; }
    public ITestServer server;

    public LoginService(ITestServer server)
    {
        this.server = server;
    }

    public void Login()
    {
        server.Login();
        IsLoggedIn = true;
    }

    public void Logout()
    {
        IsLoggedIn = false;
    }
}
