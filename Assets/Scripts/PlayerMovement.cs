using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.aKey.isPressed)
        {
            input.x = -1;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            input.x = 1;
        }

        if (Keyboard.current.wKey.isPressed)
        {
            input.y = 1;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            input.y = -1;
        }

        input = input.normalized;

        transform.position += (Vector3)(input * speed * Time.deltaTime);
    }
}