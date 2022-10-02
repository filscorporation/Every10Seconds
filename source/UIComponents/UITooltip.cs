using System.Collections;
using Steel;

namespace SteelCustom.UIComponents
{
    public class UITooltip : ScriptComponent
    {
        private static Entity current = null;

        public static Entity ShowTooltip(Skill skill, float height = 90f)
        {
            UIImage image = UI.CreateUIImage(ResourcesManager.LoadImage("ui_frame.png"), "Tooltip", null);
            image.RectTransform.AnchorMin = Vector2.One;
            image.RectTransform.AnchorMax = Vector2.One;
            image.RectTransform.Pivot = Vector2.One;
            image.RectTransform.AnchoredPosition = new Vector2(-8, -110);
            image.RectTransform.Size = new Vector2(200, height);

            UIText text = UI.CreateUIText($"Skill: {skill.Name}\n{skill.Description}", "Text", image.Entity);
            text.Color = Color.Black;
            text.TextAlignment = AlignmentType.TopLeft;
            text.TextOverflowMode = OverflowMode.WrapByWords;
            text.RectTransform.AnchorMin = Vector2.Zero;
            text.RectTransform.AnchorMax = Vector2.One;
            text.RectTransform.OffsetMin = new Vector2(12, 12);
            text.RectTransform.OffsetMax = new Vector2(12, 12);
            
            FinishShow(image.Entity);

            return image.Entity;
        }

        private static void FinishShow(Entity tooltip)
        {
            tooltip.StartCoroutine(HideTooltipRoutine(tooltip));

            if (current != null)
                HideTooltip(current);
            current = tooltip;
        }

        public static void HideTooltip(Entity tooltip)
        {
            if (tooltip == null || tooltip.IsDestroyed() || current != tooltip)
                return;
            
            current.Destroy();
            current = null;
        }

        private static IEnumerator HideTooltipRoutine(Entity tooltip)
        {
            yield return new WaitForSeconds(3.0f);
            
            HideTooltip(tooltip);
        }
    }
}