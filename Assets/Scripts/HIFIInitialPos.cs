using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HIFIInitialPos : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        GameObject Hifi = GameObject.Find("[FOR HI-FI] ToolMenu-HiFi");
		if (Hifi != null && Hifi.activeSelf) {
            player.transform.position = new Vector3(3, 3, 0);
            player.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        Destroy(this.gameObject);
    }
    
}
