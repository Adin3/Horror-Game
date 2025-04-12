using UnityEngine;

public class door_panel_script : MonoBehaviour
{
    private Animator mAnimator;
    bool state = false;
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        if (mAnimator != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!state)
                {
                    state = true;
                    mAnimator.SetBool("isOpen", true);
                }
                else
                {
                    state = false;
                    mAnimator.SetBool("isOpen", false);
                }
            }
            
        }
    }
}