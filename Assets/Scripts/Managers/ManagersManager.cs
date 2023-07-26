using UnityEngine;

public class ManagersManager : MonoBehaviour
{
    public UIManager uiManager;

    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
    }
}
