using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationTesting : MonoBehaviour
{
    Animator m_Animator;

    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        //List<Animations> animationList = new List<Animations>();

        /*
        foreach(AnimationState states in m_Animator)
        {
            animationList.Add(states.name);
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Animator.SetTrigger("takeOff");
        }


        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("flying"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Animator.SetBool("wantToIdle") = true;
            }
            Debug.Log("Flying!!!!!!!");
        }
    }
}
