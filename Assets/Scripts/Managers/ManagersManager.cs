using UnityEngine;

public class ManagersManager : MonoBehaviour
{
    public UIManager UI;
    public IKRigManager ikRig;

    void Awake()
    {
        UI = FindObjectOfType<UIManager>();
        ikRig = FindObjectOfType<IKRigManager>();
    }
}
