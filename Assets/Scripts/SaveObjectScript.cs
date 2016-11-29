using UnityEngine;

public class SaveObjectScript : MonoBehaviour, Interactable
{

    public void DoAction()
    {
        GameObject.FindGameObjectWithTag("Controller").GetComponent<SaveDataContorller>().CreateNewSave("1");
    }
}
