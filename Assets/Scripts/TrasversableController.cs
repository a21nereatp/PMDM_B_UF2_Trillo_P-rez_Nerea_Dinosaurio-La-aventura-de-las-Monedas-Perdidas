using UnityEngine;

public class TrasversableController : MonoBehaviour
{
    CompositeCollider2D compositeCollider;

    void Start()
    {
        compositeCollider = GetComponent<CompositeCollider2D>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.DownArrow) && PlayerController.isClimbing)
        {
            compositeCollider.isTrigger = true;
        }
        else
        {
            compositeCollider.isTrigger = false;
        }
    }
}
