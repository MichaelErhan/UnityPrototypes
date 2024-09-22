using Netologia.Behaviours;
using Netologia.TowerDefence;
using Netologia.TowerDefence.Behaviors;
using UnityEngine;
using Zenject;

namespace Netologia.Systems
{
    public class TowerSystem : GameObjectPoolContainer<Tower>, Director.IManualUpdate
    {
        private UnitSystem _units;              //injected
        private ProjectileSystem _projectiles;  //injected

        // Логика TowerSystem
        public void ManualUpdate()
        {
            var delta = TimeManager.DeltaTime;
            foreach (var pool in this)
            {
                foreach (var tower in pool)
                {
                    if (!tower.DecrementAttackReload(delta))
                        continue;

                    var position = tower.transform.position;

                    // Поиск новой цели, если текущая цель отсутствует
                    if (!tower.HasTarget)
                        tower.Target = _units.FindTarget(position, tower.Range);

                    // Если цели нет, пропускаем итерацию
                    if (!tower.HasTarget) continue;

                    // Получаем снаряд из пула
                    var projectile = _projectiles[tower.Projectile].Get;

                    // Добавляем проверку на null и уничтоженный объект
                    if (projectile == null || projectile.gameObject == null)
                    {
                        Debug.LogWarning("Projectile is null or has been destroyed.");
                        continue; // Пропускаем, если снаряд не существует
                    }

                    // Подготовка данных для снаряда и выполнение атаки
                    projectile.PrepareData(position, tower.Target, tower.Damage, tower.AttackElemental);
                    tower.Attack();
                }
            }
        }

        public void OnDespawnUnit(int unitID)
        {
            foreach (var pair in this)
            {
                foreach (var tower in pair)
                {
                    // Если цель башни уничтожена, сбрасываем цель
                    if (tower.TargetID == unitID)
                        tower.Target = null;
                }
            }
        }

        [Inject]
        private void Construct(UnitSystem units, ProjectileSystem projectiles)
        {
            _units = units;
            _projectiles = projectiles;
        }
    }
}
