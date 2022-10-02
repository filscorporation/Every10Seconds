using Steel;
using SteelCustom.Skills;
using SteelCustom.UIComponents;

namespace SteelCustom
{
    public class SkillObject : ScriptComponent
    {
        public Skill Skill;
        private Sprite sprite;
        private Entity tooltip;

        public void Init(Skill skill)
        {
            Skill = skill;
            
            sprite = ResourcesManager.LoadImage(Skill.SkillObjectName);
            Entity.AddComponent<SpriteRenderer>().Sprite = null;
            
            Entity effect = ResourcesManager.LoadAsepriteData("skill_spawn_effect.aseprite").CreateEntityFromAsepriteData();
            effect.Transformation.Position = Transformation.Position + new Vector3(0.0f, 0.0f, 0.5f);
            effect.GetComponent<Animator>().Play("Effect");
            effect.Destroy(1.0f);

            StartCoroutine(Coroutine.WaitForSeconds(() => GetComponent<SpriteRenderer>().Sprite = sprite, 0.5f));
            
            Entity.Destroy(Skill is Amnesia ? 30.0f : 10.0f);
            StartCoroutine(Coroutine.WaitForSeconds(() =>
                {
                    Entity effect2 = ResourcesManager.LoadAsepriteData("skill_disappear_effect.aseprite").CreateEntityFromAsepriteData();
                    effect2.Transformation.Position = Transformation.Position + new Vector3(0.0f, 0.0f, 0.6f);
                    effect2.GetComponent<Animator>().Play("Effect");
                    effect2.Destroy(0.7f);
                },
                Skill is Amnesia ? 29.7f : 9.7f
            ));
        }

        public override void OnUpdate()
        {
            if (GameManager.Player == null || GameManager.Player.Entity.IsDestroyed())
                return;
            
            if (GameManager.SkillSpawner.IsPlacingSkill || GameManager.SkillSpawner.IsRemovingSkill)
                return;
            
            if (Vector2.Distance(GameManager.Player.Transformation.Position, Transformation.Position) < 0.4f)
            {
                GameManager.SkillSpawner.TryInsertSkill(Skill);
                Entity.Destroy();
                
                Entity effect = ResourcesManager.LoadAsepriteData("skill_take_effect.aseprite").CreateEntityFromAsepriteData();
                effect.Transformation.Position = Transformation.Position + new Vector3(0.0f, 0.0f, 0.5f);
                effect.GetComponent<Animator>().Play("Effect");
                effect.Destroy(1.0f);
            
                effect.AddComponent<AudioSource>().Play(ResourcesManager.LoadAudioTrack("skill_pickup.wav"));
            }
        }

        public override void OnMouseEnter()
        {
            Log.LogInfo("Show tooltip");
        }

        public override void OnMouseExit()
        {
            UITooltip.HideTooltip(tooltip);
            tooltip = null;
        }
    }
}