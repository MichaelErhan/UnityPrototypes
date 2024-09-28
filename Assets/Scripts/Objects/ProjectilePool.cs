using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int poolSize = 256; // Размер пула
    [SerializeField] private Transform projectileParent; // Родитель для всех объектов пула

    private Queue<GameObject> projectilePool;

    private void Awake()
    {
        projectilePool = new Queue<GameObject>();

        // Проверяем, есть ли родительский объект
        if (projectileParent == null)
        {
            // Если не указан, создаем новый пустой объект как родителя
            GameObject parentObject = new GameObject("ProjectilePoolParent");
            projectileParent = parentObject.transform;
        }

        // Создаем пул объектов
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab, projectileParent); // Создаем объект как child
            obj.SetActive(false);
            projectilePool.Enqueue(obj);
        }
    }

    // Получаем пулю из пула
    public GameObject GetProjectile()
    {
        if (projectilePool.Count > 0)
        {
            GameObject projectile = projectilePool.Dequeue();
            projectile.SetActive(true);
            return projectile;
        }
        return null; // Если пул пустой
    }

    // Возвращаем пулю обратно в пул
    public void ReturnProjectile(GameObject projectile)
    {
        projectile.SetActive(false);
        projectile.transform.SetParent(projectileParent); // Назначаем родителя обратно
        projectilePool.Enqueue(projectile);
    }
}
