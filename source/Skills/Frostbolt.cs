using Steel;

namespace SteelCustom.Skills
{
    public class Frostbolt : Skill
    {
        public override string SkillObjectName => "frostbolt_object.png";
        public override string UISkillName => "ui_frostbolt.aseprite";
        public override float Duration => 1.0f;
        public override float Rarity => 0.7f;
        public override float MinTimeToSpawn => 30.0f;
        public override string Name => "Frostbolt";
        public override string Description => "Freeze enemy for 10 seconds";
        
        public override void Use()
        {
            Entity entity = ResourcesManager.LoadAsepriteData("frostbolt.aseprite").CreateEntityFromAsepriteData();

            entity.Transformation.Position = GameManager.Player.CastPoint.Position;
            entity.AddComponent<Projectile>().Init(Camera.Main.ScreenToWorldPoint(Input.MousePosition), 3.0f, OnHit);
            
            entity.AddComponent<AudioSource>().Play(ResourcesManager.LoadAudioTrack("shoot.wav"));
            entity.GetComponent<AudioSource>().Volume = 0.7f;
        }

        public override void EndUse() { }

        private void OnHit(Entity entity, Enemy enemy)
        {
            Entity effect = ResourcesManager.LoadAsepriteData("frostbolt_effect.aseprite").CreateEntityFromAsepriteData();
            effect.Transformation.Position = entity.Transformation.Position + new Vector3(0.0f, 0.0f, 0.5f);
            effect.GetComponent<Animator>().Play("Effect");
            effect.Destroy(0.4f);
            
            effect.AddComponent<AudioSource>().Play(ResourcesManager.LoadAudioTrack("ice_explosion.wav"));
            
            enemy.Freeze(10);
        }
    }
}