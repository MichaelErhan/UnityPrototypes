using Netologia.Behaviours;
using Netologia.TowerDefence.Behaviors;
using UnityEngine;

namespace Netologia.TowerDefence
{
    public class ArcherProjectile : MonoBehaviour, IPoolElement<Projectile>
    {
        private float _damage;
        private Vector3? _endPosition;
        private Unit _target;
        private ElementalType _elementalType;
        private readonly float _lifetime = 5f; // Время жизни снаряда
        private float _timer;

        [field: SerializeField]
        public float MoveSpeed { get; private set; } = 5f; // Значение по умолчанию


        public Projectile Ref { get; set; }
        public int ID { get; set; }

        // Используем либо текущую позицию цели, либо сохранённую конечную позицию
        public Vector3 TargetPosition
        {
            get
            {
                // Проверяем, не был ли уничтожен объект цели
                if (_target == null || _target.Equals(null))
                {
                    // Возвращаем конечную позицию, если цель была уничтожена
                    return _endPosition ?? Vector3.zero; // Используем Vector3.zero как fallback
                }

                // Возвращаем позицию цели, если объект существует
                return _target.transform.position;
            }
        }

        public int TargetID { get; private set; } = -1;

        // Метод для нанесения урона цели
        public void DealDamage()
        {
            if (_target == null) return;

            _target.CurrentHealth -= _damage;

            // Если здоровье моба <= 0, уничтожаем его и начисляем золото
            if (_target.CurrentHealth <= 0)
            {
                Director.Instance.AddMoney(1);
                Destroy(_target.gameObject);
                _target = null; // Убираем ссылку на уничтоженный объект
            }

            // Уничтожаем снаряд
            Destroy(gameObject);
        }

        // Обнуление цели
        public void ResetTarget()
        {
            _endPosition = null;
            _target = null;
        }

        // Подготовка данных для снаряда (позиция, цель, урон и тип)
        public void PrepareData(Vector3 position, Unit target, float damage, ElementalType type)
        {
            transform.position = position;
            _target = target;
            _damage = damage;
            _elementalType = type;
            _endPosition = target != null ? target.transform.position : (Vector3?)null;
            TargetID = target.ID;
        }

        // Движение пули
        private void Update()
        {
            if (!TimeManager.IsGame) return;

            _timer += Time.deltaTime;
            if (_timer >= _lifetime)
            {
                Destroy(gameObject); // Уничтожаем снаряд, если время жизни истекло
                return;
            }

            if (_target == null && !_endPosition.HasValue)
            {
                Destroy(gameObject); // Уничтожаем снаряд, если нет цели
                return;
            }

            Vector3 direction = (TargetPosition - transform.position).normalized;
            transform.position += direction * MoveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, TargetPosition) <= 0.2f)
            {
                DealDamage();
            }
        }

    }
}
