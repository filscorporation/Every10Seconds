using Steel;

namespace SteelCustom.Skills
{
    public class Blast : Skill
    {
        public override string SkillObjectName => "blast_object.png";
        public override string UISkillName => "ui_blast.aseprite";
        public override float Duration => 1.0f;
        public override float Rarity => 1.0f;
        public override float MinTimeToSpawn => 0.0f;
        public override string Name => "Blast";
        public override string Description => "Fire small blast dealing some damage to enemies";
        
        public override void Use()
        {
            Entity entity = ResourcesManager.LoadAsepriteData("blast.aseprite").CreateEntityFromAsepriteData();

            entity.Transformation.Position = GameManager.Player.CastPoint.Position;
            entity.AddComponent<Projectile>().Init(Camera.Main.ScreenToWorldPoint(Input.MousePosition), 3.0f, OnHit);
            
            entity.AddComponent<AudioSource>().Play(ResourcesManager.LoadAudioTrack("shoot.wav"));
            entity.GetComponent<AudioSource>().Volume = 0.7f;
        }

        public override void EndUse() { }

        private void OnHit(Entity entity, Enemy enemy)
        {
            Entity effect = ResourcesManager.LoadAsepriteData("blast_effect.aseprite").CreateEntityFromAsepriteData();
            effect.Transformation.Position = entity.Transformation.Position + new Vector3(0.0f, 0.0f, 0.5f);
            effect.GetComponent<Animator>().Play("Effect");
            effect.Destroy(0.6f);
            
            effect.AddComponent<AudioSource>().Play(ResourcesManager.LoadAudioTrack("small_explosion.wav"));
            
            enemy.TakeDamage(1);
        }
    }
}