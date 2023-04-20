using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] public PlayerController playerController;

    private Animator ani;
    float horizontalInput ;
    float verticalInput ;

    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (ani.GetBool("Crouch") == false)  //walk
        {
            if (Input.GetMouseButton(0))
            {
                ani.SetBool("FireBool", true);
            }
            else
            {
                ani.SetBool("FireBool", false);
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ani.SetBool("Crouch", true);

            }

            else
            {
                walk();
                if (Input.GetKeyDown(KeyCode.Space) && playerController.IsGrounded())
                {
                    ani.SetBool("Crouch", false);
                    ani.SetTrigger("Jump");
                }
            }
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            {
                ani.SetBool("isRunning", true);
            }
            else
            {
                ani.SetBool("isRunning", false);
            }

            if (Input.GetKey(KeyCode.R))
            {
                ani.SetTrigger("ReloadTrigger");
            }
        }
        else         // Crouch
        {
            Crouch();
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ani.SetBool("Crouch", false);
            }

            if (Input.GetMouseButton((0))&&!Input.GetKey(KeyCode.W))     // rapid Fire Mode
            {
                ani.SetBool("RapidFireCrouch", true);
            }
            else
            {
                ani.SetBool("RapidFireCrouch", false);
            }

            if (Input.GetKeyDown((KeyCode.R)))
            {
                ani.SetTrigger("CrouchReload");
            }

            if (Input.GetKey(KeyCode.W))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ani.SetBool("FireMoving",true);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    ani.SetBool("FireMoving",false);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ani.SetBool("Crouch", false);
                ani.SetTrigger("Jump");
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
