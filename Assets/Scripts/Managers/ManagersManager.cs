using UnityEngine;

public class ManagersManager : MonoBehaviour
{
    public static ManagersManager instance;
    public UIManager UI;
    public IKRigManager ikRig;

    void Awake()
    {
        instance = this;
        UI = FindObjectOfType<UIManager>();
        ikRig = FindObjectOfType<IKRigManager>();
    }
}
