using UnityEngine;

public class Teleports : MonoBehaviour
{
    [SerializeField] public Transform leftTeleporter;
    [SerializeField] public Transform rightTeleporter;
    public void Teleport(bool direction)
    {
        if (!leftTeleporter || !rightTeleporter) return;
        transform.position = (direction) ? transform.position - leftTeleporter.position : transform.position - rightTeleporter.position;
    }
}
