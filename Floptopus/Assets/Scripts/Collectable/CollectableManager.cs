using UnityEngine;
using System.Collections;

public class CollectableManager : MonoBehaviour
{
    public int totalAmountCollectables;
    CollectableUI cUI;
    int currentAmountCollectables;

	void Start ()
    {
        cUI = GameObject.FindGameObjectWithTag("CollectableUI").GetComponent<CollectableUI>();
        cUI.UpdateCollectableCount(0, totalAmountCollectables);
        currentAmountCollectables = 0;
	}

    public void CollectableFound()
    {
        currentAmountCollectables++;
        if (cUI != null)
        {
            cUI.UpdateCollectableCount(currentAmountCollectables, totalAmountCollectables);
        }
        if (currentAmountCollectables == totalAmountCollectables)
            Application.LoadLevel(0);
    }
}
