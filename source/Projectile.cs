using System;
using Steel;

namespace SteelCustom
{
    public class Projectile : ScriptComponent
    {
        private Vector3 startPosition;
        private Vector3 direction;
        private float speed;
        private Action<Entity, Enemy> onFinishAction;
        
        public void Init(Vector3 target, float speed, Action<Entity, Enemy> onFinish)
        {
            startPosition = Transformation.Position;
            this.speed = speed;
            direction = (target - startPosition).Normalize();
            onFinishAction = onFinish;
            
            Entity.Destroy(10.0f);
        }

        public override void OnUpdate()
        {
            Transformation.Position += direction * Time.DeltaTime * speed;

            foreach (Entity entity in Physics.PointCast(Transformation.Position))
            {
                Enemy enemy = entity.GetComponent<Enemy>();
                if (enemy != null)
                {
                    onFinishAction?.Invoke(Entity, enemy);
                    Entity.Destroy();
                }
            }
        }
    }
}