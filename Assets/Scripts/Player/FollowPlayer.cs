using Unity.VisualScripting;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float heightModidier;

    private void Update()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + heightModidier, transform.position.z);
    }
}
