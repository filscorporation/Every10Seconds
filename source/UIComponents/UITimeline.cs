using System.Collections.Generic;
using System.Linq;
using Steel;
using SteelCustom.Skills;

namespace SteelCustom.UIComponents
{
    public class UITimeline : ScriptComponent
    {
        public float MousePosition = -1.0f;
        
        private RectTransformation rect;
        private UIImage blur;
        private UIImage timeline;
        private UIImage progress;
        private RectTransformation skillsRoot;
        private List<UISkill> skills = new List<UISkill>();
        
        public Sprite SkillFrameSprite;
        public Sprite ActiveFrameSprite;
        public Sprite GhostFrameSprite;
        public Sprite RedFrameSprite;
        public Sprite RemoveFrameSprite;
        
        private UIImage uiGhost;
        private Skill currentPlacedSkill;

        private UISkill lastActivated;
        private UIText insertTipText;
        private UIText removeTipText;

        public override void OnCreate()
        {
            rect = GetComponent<RectTransformation>();

            SkillFrameSprite = ResourcesManager.LoadImage("ui_timeline_skill_frame.png");
            SkillFrameSprite.SetAs9Sliced(3);
            SkillFrameSprite.PixelsPerUnit = 64;
            ActiveFrameSprite = ResourcesManager.LoadImage("ui_timeline_active_frame.png");
            ActiveFrameSprite.SetAs9Sliced(7);
            ActiveFrameSprite.PixelsPerUnit = 64;
            GhostFrameSprite = ResourcesManager.LoadImage("ui_timeline_ghost_frame.png");
            GhostFrameSprite.SetAs9Sliced(7);
            GhostFrameSprite.PixelsPerUnit = 64;
            RedFrameSprite = ResourcesManager.LoadImage("ui_timeline_red_frame.png");
            RedFrameSprite.SetAs9Sliced(7);
            RedFrameSprite.PixelsPerUnit = 64;
            RemoveFrameSprite = ResourcesManager.LoadImage("ui_timeline_remove_frame.png");
            RemoveFrameSprite.SetAs9Sliced(7);
            RemoveFrameSprite.PixelsPerUnit = 64;
            
            blur = UI.CreateUIImage(ResourcesManager.LoadImage("blur.png"), "UI blur", Entity);
            blur.RectTransform.AnchorMin = Vector2.Zero;
            blur.RectTransform.AnchorMax = Vector2.One;
            blur.Color = new Color(0.0f, 0.0f, 0.0f, 0.45f);
            blur.Entity.IsActiveSelf = false;

            insertTipText = UI.CreateUIText("Insert new skill to timeline", "Tip", blur.Entity);
            insertTipText.RectTransform.AnchorMin = Vector2.Zero;
            insertTipText.RectTransform.AnchorMax = Vector2.Zero;
            insertTipText.RectTransform.AnchoredPosition = new Vector2(20, 64 * 2);
            insertTipText.RectTransform.Pivot = new Vector2(0.0f, 0.0f);
            insertTipText.RectTransform.Size = new Vector2(700, 200);
            insertTipText.Color = Color.Black;
            insertTipText.TextSize = 64;
            insertTipText.Color = Color.Black;

            removeTipText = UI.CreateUIText("Click any skill to remove it", "Tip", blur.Entity);
            removeTipText.RectTransform.AnchorMin = Vector2.Zero;
            removeTipText.RectTransform.AnchorMax = Vector2.Zero;
            removeTipText.RectTransform.AnchoredPosition = new Vector2(20, 64 * 2);
            removeTipText.RectTransform.Pivot = new Vector2(0.0f, 0.0f);
            removeTipText.RectTransform.Size = new Vector2(700, 200);
            removeTipText.Color = Color.Black;
            removeTipText.TextSize = 64;
            removeTipText.Color = Color.Black;

            UIText cancelTipText = UI.CreateUIText("Press ESC to cancel", "Tip", blur.Entity);
            cancelTipText.RectTransform.AnchorMin = new Vector2(1.0f, 0.0f);
            cancelTipText.RectTransform.AnchorMax = new Vector2(1.0f, 0.0f);
            cancelTipText.RectTransform.AnchoredPosition = new Vector2(-20, 64 * 2);
            cancelTipText.RectTransform.Pivot = new Vector2(1.0f, 0.0f);
            cancelTipText.RectTransform.Size = new Vector2(300, 100);
            cancelTipText.Color = Color.Black;
            cancelTipText.TextSize = 32;
            cancelTipText.Color = Color.Black;

            timeline = UI.CreateUIImage(ResourcesManager.LoadImage("ui_timeline.png"), "UI timeline", Entity);
            RectTransformation rt = timeline.RectTransform;
            rt.AnchorMin = new Vector2(0.5f, 0.0f);
            rt.AnchorMax = new Vector2(0.5f, 0.0f);
            rt.AnchoredPosition = new Vector2(0, 40);
            rt.Pivot = new Vector2(0.5f, 0.0f);
            rt.Size = new Vector2(640 * 2, 64 * 2);

            skillsRoot = UI.CreateUIElement("Skills root", timeline.Entity).GetComponent<RectTransformation>();
            skillsRoot.AnchorMin = Vector2.Zero;
            skillsRoot.AnchorMax = Vector2.One;
            
            progress = UI.CreateUIImage(ResourcesManager.LoadImage("ui_timeline_progress.png"), "UI progress", timeline.Entity);
            progress.RectTransform.Pivot = new Vector2(0.0f, 0.5f);
            progress.RectTransform.Size = new Vector2(5 * 2, 72 * 2);
            progress.RectTransform.AnchorMin = new Vector2(0.0f, 0.5f);
            progress.RectTransform.AnchorMax = new Vector2(0.0f, 0.5f);
            SetProgress(0.0f);
        }

        public override void OnUpdate()
        {
            if (GameManager.Player == null || GameManager.Player.Entity.IsDestroyed())
                return;
            
            MousePosition = XToProgress(Input.MousePosition.X - Screen.Width * 0.5f + 2 * 640 * 0.5f);
            
            if (uiGhost != null)
            {
                if (MousePosition >= 0.0f && MousePosition <= 10.0f)
                {
                    Timeline t = GameManager.Player.Timeline;
                    if (t.CanInsert(currentPlacedSkill, MousePosition, out float betterPosition))
                    {
                        uiGhost.Sprite = GhostFrameSprite;
                        uiGhost.RectTransform.AnchoredPosition = new Vector2(ProgressToX(betterPosition), 0.5f);
                    }
                    else
                    {
                        uiGhost.Sprite = RedFrameSprite;
                        uiGhost.RectTransform.AnchoredPosition = new Vector2(ProgressToX(MousePosition), 0.5f);
                    }
                }
            }
        }

        public void SetProgress(float value)
        {
            progress.RectTransform.AnchoredPosition = new Vector2(ProgressToX(value), 0.0f);
        }

        private float ProgressToX(float value) => 8 + value / 10.0f * (timeline.RectTransform.Size.X - 16);
        private float XToProgress(float x) => (x - 8) * 10.0f / (timeline.RectTransform.Size.X - 16);

        public void AddSkill(Skill skill, float position)
        {
            UISkill uiSkill = UI.CreateUIImage(SkillFrameSprite, "UI skill", skillsRoot.Entity).Entity.AddComponent<UISkill>();
            skills.Add(uiSkill);

            uiSkill.Init(skill);

            RectTransformation rt = uiSkill.Entity.GetComponent<RectTransformation>();
            rt.Pivot = new Vector2(0.0f, 0.5f);
            rt.Size = new Vector2(skill.Duration * 64 * 2 - 4, 56 * 2 - 4);
            rt.AnchorMin = new Vector2(0.0f, 0.5f);
            rt.AnchorMax = new Vector2(0.0f, 0.5f);
            rt.AnchoredPosition = new Vector2(ProgressToX(position), 0.5f);
        }

        public void RemoveSkill(Skill skill)
        {
            int i = skills.FindIndex(s => ReferenceEquals(s.Skill, skill));
            if (i < 0)
                return;
            
            skills[i].Entity.Destroy();
            skills.RemoveAt(i);
        }

        public void ActivateSkill(Skill skill)
        {
            lastActivated?.Deactivate();

            UISkill uiSkill = skills.FirstOrDefault(s => ReferenceEquals(s.Skill, skill));
            if (uiSkill == null)
            {
                Log.LogError($"No UI for skill {skill.GetType()} found");
                return;
            }
            
            uiSkill.Activate();
            lastActivated = uiSkill;
        }

        public void DeactivateSkill(Skill skill)
        {
            UISkill uiSkill = skills.FirstOrDefault(s => ReferenceEquals(s.Skill, skill));
            if (uiSkill == null)
            {
                Log.LogError($"No UI for skill {skill.GetType()} found");
                return;
            }
            
            uiSkill.Deactivate();
            lastActivated = null;
        }

        public void ActivateInsertSkillUI(Skill skill)
        {
            blur.Entity.IsActiveSelf = true;
            insertTipText.Entity.IsActiveSelf = true;
            removeTipText.Entity.IsActiveSelf = false;

            uiGhost = UI.CreateUIImage(GhostFrameSprite, "UI skill", skillsRoot.Entity);
            currentPlacedSkill = skill;

            RectTransformation rt = uiGhost.RectTransform;
            rt.Pivot = new Vector2(0.0f, 0.5f);
            rt.Size = new Vector2(currentPlacedSkill.Duration * 64 * 2 - 4, 56 * 2 - 4);
            rt.AnchorMin = new Vector2(0.0f, 0.5f);
            rt.AnchorMax = new Vector2(0.0f, 0.5f);
            rt.AnchoredPosition = new Vector2(ProgressToX(MousePosition), 0.5f);
        }

        public void ActivateRemoveSkillUI(Skill skill)
        {
            blur.Entity.IsActiveSelf = true;
            insertTipText.Entity.IsActiveSelf = false;
            removeTipText.Entity.IsActiveSelf = true;
        }

        public void DeactivateInsertSkillUI()
        {
            blur.Entity.IsActiveSelf = false;
            
            if (uiGhost != null)
            {
                uiGhost.Entity.Destroy();
                uiGhost = null;
            }
        }
    }
}