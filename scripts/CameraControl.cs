using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform cam;

    public string[] tags = new string[] {"scoring", "community", "loading", "field"}; // the order shows tag hierarchy, tag with lowest index is the most important
    private Vector3 defultLocation;
    private Quaternion defultRotation;

    private int level; // shos currnet level in hierarchy
    void Start()
    {
        defultLocation = new Vector3(cam.position.x, cam.position.y, cam.position.z);
        defultRotation = new Quaternion(cam.rotation.x, cam.rotation.y, cam.rotation.z, cam.rotation.w);
        level = tags.Length;
    }

    private void OnTriggerStay(Collider other) {
        // if (other.CompareTag(scoringZoneTag)) {
            
        // }
        // else if (other.CompareTag(communityAreaTag)) {

        // }
        // else if (other.CompareTag(loadingZoneTag)) {

        // }
        for (int i = tags.Length-1; i >= 0; i--) {
            string t = tags[i];
            if (other.CompareTag(t) && i <= level){
                level = i;
                setPositionToOther(other);
            }
        }
        if (level == tag.Length){
            cam.position = defultLocation;
            cam.rotation = defultRotation;
        }
    }

    private void setPositionToOther(Collider other){
        try {
            Transform temp = other.transform.GetChild(0);
            // Debug.Log("view changing to: "+ temp.name);
            cam.position = temp.position;
            cam.rotation = temp.rotation;
        }
        finally {
            //  Debug.Log($"error: bad use of tag {other.tag}");
        }
    }
    // Update is called once per frame
    void Update()
    {
        level = tags.Length; 
    }
}
