using Netologia.Behaviours;
using Netologia.TowerDefence;
using Netologia.TowerDefence.Behaviors;
using UnityEngine;
using Zenject;

namespace Netologia.Systems
{
	public class ProjectileSystem : GameObjectPoolContainer<Projectile>, Director.IManualUpdate
	{
		private EffectSystem _effects;		//injected
		
		[SerializeField, Min(0.01f)]
		private float _hitDistance = 0.3f;

        //Логика ProjectileSystem
        public void ManualUpdate()
        {
            var delta = TimeManager.DeltaTime;
            foreach (var pool in this)
            {
                foreach (var projectile in pool)
                {
                    if (projectile == null) continue;  // Проверка на null самого снаряда

                    var transform = projectile.transform;
                    var position = transform.position;
                    var targetPosition = projectile.TargetPosition;

                    // Проверка на уничтожение цели (цель может быть null)
                    if (projectile.TargetID == -1 || projectile.TargetPosition == Vector3.zero)
                    {
                        // Если цели больше нет, убираем снаряд
                        this[projectile.Ref].ReturnElement(projectile.ID);
                        continue;
                    }

                    // Логика движения снаряда
                    var direction = Vector3.Normalize(targetPosition - position);
                    position += direction * (projectile.MoveSpeed * delta);
                    transform.up = direction; // Поворачиваем снаряд в направлении движения
                    transform.position = position;

                    // Если снаряд достиг цели
                    if (Vector3.SqrMagnitude(position - targetPosition) <= _hitDistance)
                    {
                        projectile.DealDamage(); // Наносим урон

                        // Возвращаем пулю на место после попадания
                        this[projectile.Ref].ReturnElement(projectile.ID);
                    }
                }
            }
        }


        public void OnDespawnUnit(int unitID)
		{
			foreach (var pool in this)
				foreach (var projectile in pool)
					if(projectile.TargetID == unitID)
						projectile.ResetTarget();
		}

		[Inject]
		private void Construct(EffectSystem effects)
		{
			(_effects) = (effects);
			//SqrtMagnitude optimization
			_hitDistance *= _hitDistance;
		}
	}
}