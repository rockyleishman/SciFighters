using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator ani;
    float horizontalInput = Input.GetAxis("Horizontal");
    float verticalInput = Input.GetAxis("Vertical");

    // Start is called before the first frame update
    void Start()
    {
        ani= GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (ani.GetBool("Crouch") == false)  //when the state is walking, we can set it to crouch state or just use walk function
        {








            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ani.SetBool("Crouch", true);
                
            }

            else
            {
                walk();
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Run();
                }
            }
            
        }
        else  // if repress the ctrl key, set it back to walk state
        {
            Crouch();
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ani.SetBool("Crouch", false);
            }
        }
        if (ani.GetBool("Run") == false)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) //if press shift, set it to run state
            {
                ani.SetBool("Run", true);
            }

        }
        else  //if it is in running state, pressing shift set it to walk state
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                ani.SetBool("Run", false);
            }
        }


    }
    void walk()
    {
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))   //if we press WSAD key, it will make walk blend tree
        {

            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            if (horizontalInput > 1)
            {
                horizontalInput=1;
            }
            if (verticalInput > 1)
            {
                verticalInput=1;
            }
            ani.SetFloat("Horizontal", horizontalInput, 0.05f, Time.deltaTime);
            ani.SetFloat("Vertical", verticalInput, 0.05f, Time.deltaTime);
        }
        else
        {
            ani.SetFloat("Horizontal", 0, 0.05f, Time.deltaTime);   //set the movement to Idle gradually
            ani.SetFloat("Vertical", 0, 0.05f, Time.deltaTime);
        }
    }
    void Crouch()
    {
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            if (horizontalInput > 1)
            {
                horizontalInput = 1;
            }
            if (verticalInput > 1)
            {
                verticalInput = 1;
            }
            ani.SetFloat("Horizontal", horizontalInput, 0.05f, Time.deltaTime);
            ani.SetFloat("Vertical", verticalInput, 0.05f, Time.deltaTime);
        }
        else
        {
            ani.SetFloat("Horizontal", 0, 0.05f, Time.deltaTime);   //set the movement to Idle gradually
            ani.SetFloat("Vertical", 0, 0.05f, Time.deltaTime);
        }
    }
    void Run()
    {

            ani.SetTrigger("Run");

    }
}
