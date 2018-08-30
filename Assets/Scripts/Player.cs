using UnityEngine;

public class Player : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private Vector3 velocity;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * 10;
    }

    void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
    }
}