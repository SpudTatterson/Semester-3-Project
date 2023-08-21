using UnityEngine;

public class ManagersManager : MonoBehaviour
{
    public UIManager uiManager;
    public IKRigManager iKRigManager;

    void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        iKRigManager = FindObjectOfType<IKRigManager>();
    }
}
