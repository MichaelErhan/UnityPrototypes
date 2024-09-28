using Netologia.TowerDefence;
using Netologia;
using System.Collections;
using UnityEngine;


public class Archer : MonoBehaviour
{
    [SerializeField] private float attackRange = 5f; // Дальность атаки
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootDelay = 3f; // Время между выстрелами
    [SerializeField] private float moveSpeed = 2f; // Скорость движения лучника
    [SerializeField] private float shootingDistance = 3f; // Расстояние для стрельбы

    private Transform target;
    private Coroutine shootCoroutine;
    private ProjectilePool projectilePool;

    private void Start()
    {
        projectilePool = FindObjectOfType<ProjectilePool>(); // Получаем ссылку на пул пуль
    }

    private void Update()
    {
        // Если игра на паузе, лучник не должен ничего делать
        if (!TimeManager.IsGame) return;

        FindTarget();

        if (target != null)
        {
            if (IsOnPath())
            {
                // Если на тропе, ищем случайное направление
                MoveInRandomDirection();
            }
            else
            {
                // Если не на тропе, продолжаем двигаться к цели
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
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Проверяем, можем ли мы стрелять
        if (distanceToTarget <= shootingDistance)
        {
            if (shootCoroutine == null)
            {
                shootCoroutine = StartCoroutine(Shoot()); // Запускаем стрельбу
            }
        }
        else
        {
            Vector3 moveDirection = (target.position - transform.position).normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    private void MoveInRandomDirection()
    {
        Vector2 randomDirection = Random.insideUnitSphere.normalized; // Генерируем случайное направление
        Vector2 newPosition = (Vector2)transform.position + randomDirection;

        // Проверяем, свободна ли новая позиция от тропы
        if (!IsOnPathAtPosition(newPosition))
        {
            // Плавно перемещаемся к новой позиции
            transform.position = Vector2.MoveTowards(transform.position, newPosition, moveSpeed * Time.deltaTime);
        }
    }

    private bool IsOnPathAtPosition(Vector2 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 0.1f, LayerMask.GetMask("pathLayer"));
        return hit != null;
    }

    private bool IsOnPath()
    {
        // Проверяем, находится ли лучник на тропе
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("pathLayer"));
        return hit != null;
    }

    private IEnumerator Shoot()
    {
        GameObject projectileObject = projectilePool.GetProjectile(); // Получаем пулю из пула
        if (projectileObject != null)
        {
            projectileObject.transform.position = shootPoint.position;
            projectileObject.transform.rotation = shootPoint.rotation;
            projectileObject.SetActive(true);

            ArcherProjectile projectile = projectileObject.GetComponent<ArcherProjectile>();

            Unit targetUnit = target != null ? target.GetComponent<Unit>() : null;

            if (targetUnit != null && projectile != null)
            {
                projectile.PrepareData(shootPoint.position, targetUnit, 3f, ElementalType.Physic);
            }
        }
        else
        {
            Debug.LogError("Failed to shoot: no available projectiles.");
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

    //Я пытался вот так сделать методы FindGameObjectsWithTag и GetComponent , но лучник их не видит изначально, а если через Update вызывать FindTarget, то всё норм
    //Это сильно критично?

    //private List<Unit> mobs = new List<Unit>();

    //private void Awake()
    //{
    //    // Ищем всех мобов один раз и кэшируем их
    //    GameObject[] mobObjects = GameObject.FindGameObjectsWithTag("Mob");
    //    foreach (GameObject mobObject in mobObjects)
    //    {
    //        Unit mobUnit = mobObject.GetComponent<Unit>();
    //        if (mobUnit != null)
    //        {
    //            mobs.Add(mobUnit);
    //        }
    //    }
    //}

    //private void FindTarget()
    //{
    //    float shortestDistance = Mathf.Infinity;
    //    Unit nearestMob = null;

    //    foreach (Unit mobUnit in mobs)
    //    {
    //        if (mobUnit.CurrentHealth > 0) // Проверяем, жив ли моб
    //        {
    //            float distanceToMob = Vector3.Distance(transform.position, mobUnit.transform.position);
    //            if (distanceToMob < shortestDistance && distanceToMob <= attackRange)
    //            {
    //                shortestDistance = distanceToMob;
    //                nearestMob = mobUnit;
    //            }
    //        }
    //    }

    //    target = nearestMob != null ? nearestMob.transform : null;
    //}
}
