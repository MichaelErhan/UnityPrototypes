using System.Collections;
using UnityEngine;

public class Barracks : MonoBehaviour
{
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 20f;

    private void Start()
    {
        StartCoroutine(SpawnArcher());
    }

    private IEnumerator SpawnArcher()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            Instantiate(archerPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}