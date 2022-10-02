using Steel;

namespace SteelCustom.Skills
{
    public class Shockwave : Skill
    {
        public override string SkillObjectName => "shockwave_object.png";
        public override string UISkillName => "ui_shockwave.aseprite";
        public override float Duration => 1.5f;
        public override float Rarity => 0.15f;
        public override float MinTimeToSpawn => 20.0f;
        public override string Name => "Shockwave";
        public override string Description => "Send a shockwave pushing enemies from you";
        
        public override void Use()
        {
            foreach (Enemy enemy in Component.FindAllOfType<Enemy>())
            {
                Vector2 impulse = (enemy.Transformation.Position - GameManager.Player.Transformation.Position).Normalize() * 500.0f;
                enemy.Entity.GetComponent<RigidBody>().ApplyForce(impulse);
            }
            
            Entity effect = ResourcesManager.LoadAsepriteData("shockwave_effect.aseprite").CreateEntityFromAsepriteData();
            effect.Transformation.Position = GameManager.Player.Transformation.Position + new Vector3(0.0f, 0.0f, 0.4f);
            effect.GetComponent<Animator>().Play("Effect");
            effect.Destroy(0.8f);
            
            effect.AddComponent<AudioSource>().Play(ResourcesManager.LoadAudioTrack("shock_wave.wav"));
        }

        public override void EndUse() { }
    }
}