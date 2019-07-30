using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Core.Services;

public class Tester : MonoBehaviour
{
    void Start()
    {
        ITestServer server = ServiceLocator.Instance.Get<LoginService>().server;
        server.Login();
    }
}
