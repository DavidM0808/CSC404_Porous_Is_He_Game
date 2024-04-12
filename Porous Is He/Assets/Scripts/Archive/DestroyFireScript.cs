using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFireScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        other.gameObject.SetActive(false);
        //Destroy(other.gameObject);

        //if want water drop to be gone
        Destroy(this.gameObject);
 
    }
}
