namespace SteelCustom.Skills
{
    public class Amnesia : Skill
    {
        public override string SkillObjectName => "amnesia_object.png";
        public override string UISkillName => "ui_amnesia.aseprite";
        public override float Duration => 10.0f;
        public override float Rarity => 0.5f;
        public override float MinTimeToSpawn => 45.0f;
        public override string Name => "Amnesia";
        public override string Description => "Forget one skill you learned";
        
        public override void Use() { }

        public override void EndUse() { }
    }
}