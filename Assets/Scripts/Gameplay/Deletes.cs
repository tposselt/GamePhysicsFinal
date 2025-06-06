using UnityEngine;

public class Deletes : MonoBehaviour
{
    [SerializeField] string deleteTag;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == deleteTag) Destroy(gameObject);
    }
}
