using UnityEngine;
using System.Collections;

public class ARAnchor : MonoBehaviour {

    public GameObject anchorableObject;
    BoardAnchor boardAnchor;

	// Use this for initialization
	void Awake () {
        boardAnchor = anchorableObject.GetComponent<BoardAnchor>();
        if(boardAnchor == null)
        {
            Debug.LogError("Invalide BoardAnchor on ARAnchor for " + gameObject.name);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Place()
    {
        boardAnchor.Place( gameObject);
    }
}
