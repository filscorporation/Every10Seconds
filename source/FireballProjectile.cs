using System;
using Steel;
using Math = Steel.Math;

namespace SteelCustom
{
    public class FireballProjectile : ScriptComponent
    {
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private float timer;
        private float duration;
        private Action<Entity> onFinishAction;
        
        public void Init(Vector3 target, float speed, Action<Entity> onFinish)
        {
            startPosition = Transformation.Position.SetZ(1.5f);
            targetPosition = target.SetZ(1.5f);
            duration = Vector3.Distance(startPosition, targetPosition) / speed;
            onFinishAction = onFinish;
        }

        public override void OnUpdate()
        {
            timer += Time.DeltaTime;
            Transformation.Position = Math.Lerp(startPosition, targetPosition, timer / duration);

            if (timer > duration)
            {
                Entity.Destroy();
                onFinishAction?.Invoke(Entity);
            }
        }
    }
}