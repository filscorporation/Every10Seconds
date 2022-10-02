using Steel;

namespace SteelCustom.Skills
{
    public class Fireball : Skill
    {
        public override string SkillObjectName => "fireball_object.png";
        public override string UISkillName => "ui_fireball.aseprite";
        public override float Duration => 2.0f;
        public override float Rarity => 0.2f;
        public override float MinTimeToSpawn => 30.0f;
        public override string Name => "Fireball";
        public override string Description => "Deal damage in the area";

        private const float BLAST_RADIUS = 0.8f;
        
        public override void Use()
        {
            Entity entity = new Entity("Fireball", GameManager.Map.Entity);

            entity.AddComponent<SpriteRenderer>().Sprite = ResourcesManager.LoadImage("fireball.png");

            entity.Transformation.Position = GameManager.Player.CastPoint.Position.SetZ(0.0f);
            Vector3 target = Camera.Main.ScreenToWorldPoint(Input.MousePosition);
            entity.AddComponent<FireballProjectile>().Init(target, 2.0f, OnHit);
            
            entity.AddComponent<AudioSource>().Play(ResourcesManager.LoadAudioTrack("shoot.wav"));
            entity.GetComponent<AudioSource>().Volume = 0.7f;
        }

        public override void EndUse() { }

        private void OnHit(Entity entity)
        {
            Entity effect = ResourcesManager.LoadAsepriteData("explosion_effect.aseprite").CreateEntityFromAsepriteData();
            effect.Transformation.Position = entity.Transformation.Position + new Vector3(0.0f, 0.0f, 0.5f);
            effect.GetComponent<Animator>().Play("Effect");
            effect.Destroy(0.8f);
            
            effect.AddComponent<AudioSource>().Play(ResourcesManager.LoadAudioTrack("explosion.wav"));

            foreach (Enemy enemy in Component.FindAllOfType<Enemy>())
            {
                if (Vector2.Distance(enemy.Transformation.Position, entity.Transformation.Position) <= BLAST_RADIUS)
                    enemy.TakeDamage(1);
            }
        }
    }
}