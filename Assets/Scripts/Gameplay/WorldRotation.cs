using UnityEngine;

public class WorldRotation : MonoBehaviour
{
    float timer = 0;
    Rigidbody2D rb;

    void Awake()
    {
        timer = Random.Range(2f, 5f);
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            rb.AddTorque(Random.Range(100f, 200f) * (Random.Range(0, 2) == 0 ? -1 : 1), ForceMode2D.Impulse);
            timer = Random.Range(2f, 5f);
        }
    }

    public void ResetRotation()
    {
        timer = Random.Range(2f, 5f);
        rb.angularVelocity = 0;
        rb.rotation = 0;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
