using Netologia.TowerDefence.Behaviors;
using Netologia.TowerDefence;
using Netologia;
using System.Collections;
using UnityEngine;

public class ArcherProjectile : MonoBehaviour
{
    private float _damage;
    private Unit _target;
    private Vector3? _endPosition;
    private ElementalType _elementalType = ElementalType.Physic;
    private float _lifetime = 3f; // Время жизни снаряда
    private float _timer;

    [SerializeField] private float moveSpeed = 5f; // Скорость полета пули

    // Подготовка данных для снаряда (позиция, цель, урон и тип)
    public void PrepareData(Vector3 position, Unit target, float damage, ElementalType type)
    {
        transform.position = position;
        _target = target;
        _damage = damage;
        _elementalType = type;
        _endPosition = target != null ? target.transform.position : (Vector3?)null;
        _timer = 0f; // Сброс таймера жизни снаряда
    }

    private void Update()
    {
        if (!TimeManager.IsGame) return;

        _timer += Time.deltaTime;
        if (_timer >= _lifetime)
        {
            DeactivateProjectile(); // Деактивируем пулю, если время жизни истекло
            return;
        }

        if (_target == null && !_endPosition.HasValue)
        {
            DeactivateProjectile(); // Деактивируем пулю, если нет цели
            return;
        }

        Vector3 direction = (TargetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, TargetPosition) <= 0.2f)
        {
            DealDamage(); // Наносим урон, если пуля достигла цели
        }
    }

    // Возвращаем текущую позицию цели или конечную позицию, если цель уничтожена
    private Vector3 TargetPosition
    {
        get
        {
            if (_target == null || _target.Equals(null))
            {
                return _endPosition ?? Vector3.zero; // Возвращаем конечную позицию или Vector3.zero
            }
            return _target.transform.position;
        }
    }

    // Метод для нанесения урона цели
    private void DealDamage()
    {
        if (_target == null) return;

        _target.CurrentHealth -= _damage;

        // Если здоровье цели меньше или равно 0, уничтожаем цель
        if (_target.CurrentHealth <= 0)
        {
            Director.Instance.AddMoney(1); // Добавляем золото игроку
            Destroy(_target.gameObject);
        }

        DeactivateProjectile(); // Деактивируем пулю после нанесения урона
    }

    // Метод для деактивации пули (возврат в пул)
    private void DeactivateProjectile()
    {
        gameObject.SetActive(false);
    }
}
