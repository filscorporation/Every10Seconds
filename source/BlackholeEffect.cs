using Steel;

namespace SteelCustom
{
    public class BlackholeEffect : ScriptComponent
    {
        private float timer = DELAY / 2;
        private const float DELAY = 0.7f;
        private const float RANGE = 4f;
        
        public override void OnUpdate()
        {
            timer += Time.DeltaTime;
            if (timer < DELAY)
                return;
            timer = 0.0f;

            Entity.AddComponent<AudioSource>().Play(ResourcesManager.LoadAudioTrack("blackhole.wav"));
            
            foreach (Enemy enemy in FindAllOfType<Enemy>())
            {
                if (Vector3.Distance(Transformation.Position, enemy.Transformation.Position) < RANGE)
                    enemy.Body.ApplyForce((Transformation.Position - enemy.Transformation.Position) * 40);
            }
        }
    }
}