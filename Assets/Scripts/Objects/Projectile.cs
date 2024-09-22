using Netologia.Behaviours;
using UnityEngine;

namespace Netologia.TowerDefence
{
    public class Projectile : MonoBehaviour, IPoolElement<Projectile>
    {
        private float _damage;
        private Vector3? _endPosition;
        private Unit _target;

        private ElementalType _elementalType;
     
        [field: SerializeField]
        public float MoveSpeed { get; private set; }

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


        public void DealDamage()
        {
            if (_endPosition.HasValue) return;

            _target.CurrentHealth -= _damage;
            _target.TryAddEffect(TimeManager.Time, _elementalType);
        }

        public void ResetTarget()
            => (_endPosition, _target) = (_target.transform.position, null);

        public void PrepareData(Vector3 position, Unit target, float damage, ElementalType type)
        {
            transform.position = position;
            (_target, _damage, _elementalType, _endPosition) = (target, damage, type, null);
            TargetID = target.ID;
        }
    }
}