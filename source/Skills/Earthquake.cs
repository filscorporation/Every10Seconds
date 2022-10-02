using Steel;

namespace SteelCustom.Skills
{
    public class Earthquake : Skill
    {
        public override string SkillObjectName => "earthquake_object.png";
        public override string UISkillName => "ui_earthquake.aseprite";
        public override float Duration => 3.0f;
        public override float Rarity => 0.2f;
        public override float MinTimeToSpawn => 45.0f;
        public override string Name => "Earthquake";
        public override string Description => "Destroy all frozen enemies";
        
        public override void Use()
        {
            Camera.Main.GetComponent<CameraController>().Shake(5);

            Entity entity = new Entity("Effect", GameManager.Map.Entity);
            entity.AddComponent<AudioSource>().Play(ResourcesManager.LoadAudioTrack("earthquake.wav"));

            foreach (Enemy enemy in Component.FindAllOfType<Enemy>())
            {
                if (enemy.IsFrozen)
                    enemy.Die();
            }
        }

        public override void EndUse() { }
    }
}