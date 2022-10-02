namespace SteelCustom
{
    public abstract class Skill
    {
        public abstract string SkillObjectName { get; }
        public abstract string UISkillName { get; }
        public abstract float Duration { get; }
        public abstract float Rarity { get; }
        public abstract float MinTimeToSpawn { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        
        public abstract void Use();
        public abstract void EndUse();
    }
}