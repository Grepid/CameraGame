using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerController.instance.acceptingInputs)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PlayerController.instance.UIModeToggle();
                print("Untoggled");
            }
        }
    }
}
