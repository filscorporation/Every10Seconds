using Steel;

namespace SteelCustom.Skills
{
    public class Blackhole : Skill
    {
        public override string SkillObjectName => "blackhole_object.png";
        public override string UISkillName => "ui_blackhole.aseprite";
        public override float Duration => 3.0f;
        public override float Rarity => 0.2f;
        public override float MinTimeToSpawn => 30.0f;
        public override string Name => "Black hole";
        public override string Description => "Create a black hole grouping enemies together";
        
        public override void Use()
        {
            Entity entity = ResourcesManager.LoadAsepriteData("blackhole_effect.aseprite", true).CreateEntityFromAsepriteData();
            entity.Parent = GameManager.Map.Entity;
            entity.Transformation.Position = Camera.Main.ScreenToWorldPoint(Input.MousePosition);
            entity.Transformation.Position = entity.Transformation.Position.SetZ(-0.1f);
            entity.AddComponent<BlackholeEffect>();
            entity.GetComponent<Animator>().Play("Effect");
            entity.Destroy(3.0f);
        }

        public override void EndUse() { }
    }
}