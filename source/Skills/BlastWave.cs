using Steel;

namespace SteelCustom.Skills
{
    public class BlastWave : Skill
    {
        public override string SkillObjectName => "blastwave_object.png";
        public override string UISkillName => "ui_blastwave.aseprite";
        public override float Duration => 3.0f;
        public override float Rarity => 0.15f;
        public override float MinTimeToSpawn => 30.0f;
        public override string Name => "Blast wave";
        public override string Description => "Fire many blasts, increased for every blast picked this game";

        private const int BASE_AMOUNT = 3;
        
        public override void Use()
        {
            for (int i = 0; i < BASE_AMOUNT + GameManager.Player.Timeline.BlastsLearned * 3; i++)
            {
                Entity entity = ResourcesManager.LoadAsepriteData("blast.aseprite").CreateEntityFromAsepriteData();

                entity.Transformation.Position = GameManager.Player.CastPoint.Position;
                entity.AddComponent<Projectile>().Init(GameManager.Player.Transformation.Position + GameManager.RandomPointOnCircle(1.0f), 3.0f, OnHit);
            }
        }

        public override void EndUse() { }

        private void OnHit(Entity entity, Enemy enemy)
        {
            Entity effect = ResourcesManager.LoadAsepriteData("blast_effect.aseprite").CreateEntityFromAsepriteData();
            effect.Transformation.Position = entity.Transformation.Position + new Vector3(0.0f, 0.0f, 0.5f);
            effect.GetComponent<Animator>().Play("Effect");
            effect.Destroy(0.6f);
            
            enemy.TakeDamage(1);
        }
    }
}