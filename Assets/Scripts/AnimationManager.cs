using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    Animator animator;
    int Horizontal;
    int Vertical;

    private float moveAmount;
    void Start()
    {
        animator = GetComponent<Animator>();
        Horizontal = Animator.StringToHash("Horizontal");
        Vertical = Animator.StringToHash("Vertical");
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        moveAmount = Mathf.Clamp01(Mathf.Abs(x) + Mathf.Abs(y));
        UpdateAnimatorValues(0, moveAmount);
    }

    void UpdateAnimatorValues(float horizontalMovement,float verticalMovement)
    {
        float snappedHorizontal;
        float snappedVertical;

        #region SNAPPED HORIZONTAL
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1f;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalMovement < 0.55f)
        {
            snappedHorizontal = -1f;
        }
        else
        {
            snappedHorizontal = 0f;
        }
        #endregion
        #region SNAPPED VERTICAL
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1f;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < 0.55f)
        {
            snappedVertical = -1f;
        }
        else
        {
            snappedVertical = 0f;
        }
        #endregion

        animator.SetFloat(Horizontal, snappedHorizontal,0.1f,Time.deltaTime);
        animator.SetFloat(Vertical, snappedVertical,0.1f,Time.deltaTime);
    }
}
