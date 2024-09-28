using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherPool : MonoBehaviour
{
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private int poolSize = 30; // Размер пула
    [SerializeField] private Transform archerParent; // Родитель для лучников
    [SerializeField] private Transform spawnPoint; // Точка спавна лучников
    [SerializeField] private float spawnInterval = 5f; // Интервал спавна

    private Queue<GameObject> archerPool;

    private void Awake()
    {
        archerPool = new Queue<GameObject>();

        // Проверяем, есть ли родительский объект
        if (archerParent == null)
        {
            // Если не указан, создаем новый пустой объект как родителя
            GameObject parentObject = new GameObject("ArcherPoolParent");
            archerParent = parentObject.transform;
        }

        // Создаем пул объектов
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(archerPrefab, archerParent); // Создаем объект как дочерний
            obj.SetActive(false);
            archerPool.Enqueue(obj);
        }

        // Запускаем процесс спавна
        StartCoroutine(SpawnArcher());
    }

    // Получаем лучника из пула
    public GameObject GetArcher()
    {
        if (archerPool.Count > 0)
        {
            GameObject archer = archerPool.Dequeue();
            archer.SetActive(true);
            return archer;
        }
        return null; // Если пул пустой
    }

    // Возвращаем лучника обратно в пул
    public void ReturnArcher(GameObject archer)
    {
        archer.SetActive(false);
        archer.transform.SetParent(archerParent); // Назначаем родителя обратно
        archerPool.Enqueue(archer);
    }

    // Процесс спавна лучников
    private IEnumerator SpawnArcher()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            GameObject archer = GetArcher();
            if (archer != null)
            {
                // Устанавливаем позицию лучника на точке спавна
                archer.transform.position = spawnPoint.position;
                archer.transform.rotation = spawnPoint.rotation;
            }
            else
            {
                Debug.LogWarning("No archers available in pool.");
            }
        }
    }
}
