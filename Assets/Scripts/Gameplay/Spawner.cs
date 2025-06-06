using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    float timer = 0;
    bool unobstructed = true;
    int obstructions;
    string[] objectTags = { "Spawnable", "Laser", "magnetObject" };

    [SerializeField] List<GameObject> spawnItems;
    [SerializeField] Transform leftTeleporter;
    [SerializeField] Transform rightTeleporter;

    void FixedUpdate()
    {
        if (unobstructed) timer -= Time.deltaTime;
        if (timer <= 0)
        {
            GameObject obj = Instantiate(spawnItems[Random.Range(0, spawnItems.Count)], transform.position, transform.rotation, transform);
            Teleports teleports;
            if (obj.TryGetComponent<Teleports>(out teleports))
            {
                teleports.leftTeleporter = leftTeleporter;
                teleports.rightTeleporter = rightTeleporter;
            }

            timer = Random.Range(0.5f, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (objectTags.Contains(collision.tag))
        {
            unobstructed = false;
            obstructions++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (objectTags.Contains(collision.tag))
        {
            obstructions--;
            if (obstructions <= 0) unobstructed = true;
        }
    }
}
