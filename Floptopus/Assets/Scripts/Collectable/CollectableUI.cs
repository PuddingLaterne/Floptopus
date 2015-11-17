using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CollectableUI : MonoBehaviour
{
    Text text;
    int current;
    int total;

	void Start ()
    {
        text = GetComponent<Text>();
	}

    void Update()
    {
        text.text = current + "/" + total;
    }
	
	public void UpdateCollectableCount(int current, int total)
    {
        this.current = current;
        this.total = total;
    }
}
