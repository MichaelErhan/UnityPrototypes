using Netologia;
using Netologia.TowerDefence;
using System.Collections;
using UnityEngine;

public class Archer : MonoBehaviour
{
    [SerializeField] private float attackRange = 5f; // Дальность атаки
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootDelay = 3f; // Время между выстрелами
    [SerializeField] private LayerMask pathLayer; // Слой для тропы мобов
    [SerializeField] private float moveSpeed = 2f; // Скорость движения лучника
    [SerializeField] private float shootingDistance = 3f; // Расстояние для стрельбы

    private Transform target;
    private Coroutine shootCoroutine;

    private void Update()
    {
        // Если игра на паузе, лучник не должен ничего делать
        if (!TimeManager.IsGame) return;

        FindTarget();

        if (target != null)
        {
            if (!IsOnPath())
            {
                MoveTowardsTarget();
            }
        }
    }

    private void FindTarget()
    {
        GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestMob = null;

        foreach (GameObject mob in mobs)
        {
            Unit mobUnit = mob.GetComponent<Unit>();
            if (mobUnit != null && mobUnit.CurrentHealth > 0) // Проверяем, жив ли моб
            {
                float distanceToMob = Vector3.Distance(transform.position, mob.transform.position);
                if (distanceToMob < shortestDistance && distanceToMob <= attackRange)
                {
                    shortestDistance = distanceToMob;
                    nearestMob = mob;
                }
            }
        }

        target = nearestMob != null ? nearestMob.transform : null;
    }


    private void MoveTowardsTarget()
    {
        if (target == null) return; // Выходим, если нет цели

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Проверяем, находимся ли мы на расстоянии для стрельбы
        if (distanceToTarget > shootingDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime; // Двигаем лучника к цели
        }
        else if (shootCoroutine == null)
        {
            shootCoroutine = StartCoroutine(Shoot()); // Запускаем стрельбу
        }
    }


    private IEnumerator Shoot()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        ArcherProjectile projectile = projectileObject.GetComponent<ArcherProjectile>();

        Unit targetUnit = target != null ? target.GetComponent<Unit>() : null;

        if (targetUnit != null && projectile != null)
        {
            projectile.PrepareData(shootPoint.position, targetUnit, 3f, ElementalType.Physic);
        }
        else
        {
            Debug.LogError("Failed to shoot: target or projectile is null.");
        }

        yield return new WaitForSeconds(shootDelay);
        shootCoroutine = null; // Освобождаем корутину
    }
    private void StopShooting()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }

    private bool IsOnPath()
    {
        // Проверяем, находится ли лучник на тропе мобов
        return Physics.CheckSphere(transform.position, 0.5f, pathLayer);
    }
}
