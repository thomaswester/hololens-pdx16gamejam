﻿using UnityEngine;
using System.Collections;


using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;

public class BoardAnchor : MonoBehaviour {

    //unique id for the cube
    public string BoardId;
    
    //persistant location storage
    WorldAnchorStore anchorStore;
    
    // Use this for initialization
    void Start()
    {
        WorldAnchorStore.GetAsync(AnchorStoreReady);

    }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;

        Debug.Log("Find Anchor for " + BoardId);
        string[] ids = anchorStore.GetAllIds();
        for (int index = 0; index < ids.Length; index++)
        {
            Debug.Log(ids[index]);
            if (ids[index] == BoardId)
            {
                WorldAnchor wa = anchorStore.Load(ids[index], gameObject);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Place( GameObject matchMe )
    {

        Debug.Log("Place");


        if (anchorStore == null)
        {
            Debug.LogError("anchorStore is null");
            return;
        }


        WorldAnchor anchor = gameObject.GetComponent<WorldAnchor>();
        //delete any previous state
        if (anchor != null)
        {
            DestroyImmediate(anchor);
            string[] ids = anchorStore.GetAllIds();
            for (int index = 0; index < ids.Length; index++)
            {
                Debug.Log(ids[index]);
                if (ids[index] == BoardId)
                {
                    bool deleted = anchorStore.Delete(ids[index]);
                    Debug.Log("deleted: " + deleted);
                    break;
                }
            }
        }

        Vector3 pos = matchMe.transform.position;
        pos.x += 0.2f;

        gameObject.transform.position = pos;
        
        anchor = gameObject.AddComponent<WorldAnchor>();
        if (anchor.isLocated)
        {
            Debug.Log("Saving persisted position immediately");
            bool saved = anchorStore.Save(BoardId, anchor);
            Debug.Log("saved: " + saved);
        }
        else
        {
            anchor.OnTrackingChanged += Anchor_OnTrackingChanged;
        }
    }

    private void Anchor_OnTrackingChanged(WorldAnchor self, bool located)
    {
        if (located)
        {
            Debug.Log("Saving persisted position in callback");
            bool saved = anchorStore.Save(BoardId, self);
            Debug.Log("saved: " + saved);
            self.OnTrackingChanged -= Anchor_OnTrackingChanged;
        }
    }
}
