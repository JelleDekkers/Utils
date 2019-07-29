using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.ServiceSystem;

public class Tester : MonoBehaviour
{
    void Start()
    {
        ITestServer server = ServiceLocator.Instance.Get<LoginService>().server;
        server.Login();
    }
}
