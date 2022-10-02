using System.Collections;
using Steel;

namespace SteelCustom
{
    public class CameraController : ScriptComponent
    {
        private float z;
        
        private Coroutine shakeRoutine;
        
        private const float MAX_OFFSET = 0.5F;
        private const float SHAKE_REDUCTION = 0.6F;

        private float offsetX;
        private float offsetY;

        public override void OnCreate()
        {
            z = Transformation.Position.Z;
        }

        public override void OnUpdate()
        {
            if (GameManager.Player == null || GameManager.Player.Entity.IsDestroyed())
                return;
            
            Vector3 target = GameManager.Player.Transformation.Position + new Vector3(0.0f, -0.4f) + new Vector3(offsetX, offsetY, Transformation.Position.Z);;
            Transformation.Position = Math.Lerp(Transformation.Position, target, 1.0f * Time.DeltaTime).SetZ(z);
        }
        
        public void Shake(float strength)
        {
            if (shakeRoutine != null)
                StopCoroutine(shakeRoutine);
            
            shakeRoutine = StartCoroutine(ShakeRoutine(strength));
        }

        private IEnumerator ShakeRoutine(float strength)
        {
            float seed = Random.NextFloat(0, 1);
            float timePassed = 0.0f;

            while (strength > 0.0f)
            {
                offsetX = MAX_OFFSET * strength * Random.PerlinNoise(seed + 0.1F, timePassed * 1000f);
                offsetY = MAX_OFFSET * strength * Random.PerlinNoise(seed + 0.2F, timePassed * 1000f);
                
                timePassed += Time.DeltaTime;
                strength -= Time.DeltaTime * SHAKE_REDUCTION;

                yield return null;
            }
        }
    }
}