using UnityEngine;

public class FPSController : MonoBehaviour
{
    public int targetFPS = 60;

    void Start()
    {
        Application.targetFrameRate = targetFPS;
    }
}
