using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR.InteractionSystem;



public class Shield : MonoBehaviour
{
    [SerializeField] Hand shieldHand;
    // Start is called before the first frame update
    void Start()
    {
        transform.parent = shieldHand.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
