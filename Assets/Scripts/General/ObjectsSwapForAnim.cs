using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectsSwapForAnim : MonoBehaviour 
{
    [SerializeField]
    private List<GameObject> firstList;
    [SerializeField]
    private List<GameObject> secondList;

    void Awake()
    {
#if UNITY_EDITOR
        foreach (GameObject element in firstList)
        {
            if (element == null)
            {
                Debug.LogError("[ObjectsSwapForAnim] There is a null element on the firstList of " + this.name, this);
            }
        }
#endif

        // The models of the second list needs to start deactivated
        foreach(GameObject toDeactivate in secondList)
        {
            if (toDeactivate != null)
            {
                toDeactivate.SetActive(false);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("[ObjectsSwapForAnim] There is a null element on the secondList of " + this.name, this);
            }
#endif
        }
    }
    
    /// <summary>
    /// This function is called by an animation event and swaps the models set on the two lists.
    /// </summary>
    private void SwapModels()
    {
        //string debugMessage = "On SwapModels: Deactivating ";

        // Deactivate models of the first list
        foreach (GameObject toDeactivate in firstList)
        {
            if (toDeactivate != null)
            {
                //debugMessage += toDeactivate.name + " | ";
                toDeactivate.SetActive(false);
            }
        }

        //debugMessage += "/n Activating: ";

        // And Activate models of the second list
        foreach (GameObject toActivate in secondList)
        {
            if (toActivate != null)
            {
                //debugMessage += toActivate.name + " | ";
                toActivate.SetActive(true);
            }
        }

        //Debug.Log(debugMessage);
    }
}
