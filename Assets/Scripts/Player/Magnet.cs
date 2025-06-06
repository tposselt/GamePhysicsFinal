using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    List<Transform> magnetObjects = new List<Transform>();
    string magnetTag = "magnetObject";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == magnetTag)
        {
            magnetObjects.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == magnetTag)
        {
            magnetObjects.Remove(collision.transform);
        }
    }

    public Vector3 CalculateMagnetPull()
    {
        Vector3 returnValue = Vector3.zero;
        foreach (var magnet in magnetObjects)
        {
            Vector3 vectorToMagnet = (magnet.position - transform.position);

            float angleToMagnet = Mathf.Rad2Deg * Mathf.Atan2(vectorToMagnet.y, vectorToMagnet.x);
            if (angleToMagnet < 0) angleToMagnet += 360;

            float forwardAngle = (transform.eulerAngles.z - 90 + 360) % 360;
            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(forwardAngle, angleToMagnet));
            angleDifference = Mathf.Max(angleDifference, 1f);

            float squareDistance = vectorToMagnet.sqrMagnitude;
            if (squareDistance < 0.01f) continue;

            float magnitude = 1f / squareDistance;// / angleDifference;

            returnValue += vectorToMagnet.normalized * magnitude;
        }

        return returnValue;
    }
}
