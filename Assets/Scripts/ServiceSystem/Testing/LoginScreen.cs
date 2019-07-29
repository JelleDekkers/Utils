using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private Button loginButton;
#pragma warning restore CS0649

    private LoginService loginService;

    public void InjectDependencies(LoginService loginService)
    {
        this.loginService = loginService;
    }

    private void OnEnable()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveListener(OnLoginButtonClicked);
    }

    private void OnLoginButtonClicked()
    {
        loginService.Login();
    }
}
