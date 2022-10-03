using System.Linq;
using Steel;

namespace SteelCustom.UIComponents
{
    public class UISkill : ScriptComponent
    {
        public Skill Skill;
        private Sprite saveSprite;
        private Entity tooltip;

        public void Init(Skill skill)
        {
            Skill = skill;

            AsepriteData ad = ResourcesManager.LoadAsepriteData(skill.UISkillName, true);
            Sprite sprite = ad.Sprites.First();
            UIImage image = UI.CreateUIImage(ad.Sprites.First(), "Icon", Entity);
            image.ConsumeEvents = false;

            image.RectTransform.AnchorMin = new Vector2(0.5f, 0.5f);
            image.RectTransform.AnchorMax = new Vector2(0.5f, 0.5f);
            float w = sprite.Width / sprite.Height * (56 * 2 - 4);
            image.RectTransform.Size = new Vector2(w, 56 * 2 - 4);
        }

        public void Activate()
        {
            GetComponent<UIImage>().Sprite = GameManager.Player.Timeline.UITimeline.ActiveFrameSprite;
        }

        public void Deactivate()
        {
            GetComponent<UIImage>().Sprite = GameManager.Player.Timeline.UITimeline.SkillFrameSprite;
        }

        public override void OnMouseEnterUI()
        {
            if (GameManager.SkillSpawner.IsRemovingSkill)
            {
                saveSprite = GetComponent<UIImage>().Sprite;
                GetComponent<UIImage>().Sprite = GameManager.Player.Timeline.UITimeline.RemoveFrameSprite;

                GameManager.SkillSpawner.CurrentSkillToRemove = Skill;
            }
        }

        public override void OnMouseOverUI()
        {
            tooltip = UITooltip.ShowTooltip(Skill);
        }

        public override void OnMouseExitUI()
        {
            UITooltip.HideTooltip(tooltip);
            tooltip = null;
            
            if (saveSprite != null)
                GetComponent<UIImage>().Sprite = saveSprite;

            if (GameManager.SkillSpawner.IsRemovingSkill)
                GameManager.SkillSpawner.CurrentSkillToRemove = null;
        }
    }
}