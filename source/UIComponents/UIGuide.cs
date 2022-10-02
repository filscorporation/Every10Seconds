using Steel;

namespace SteelCustom.UIComponents
{
    public class UIGuide : ScriptComponent
    {
        private UIText guideText;
        
        public override void OnCreate()
        {
            GetComponent<RectTransformation>().AnchorMin = Vector2.Zero;
            GetComponent<RectTransformation>().AnchorMin = Vector2.One;
            
            guideText = UI.CreateUIText("Use WASD to move, try to find new spells", "Tip", Entity);
            guideText.RectTransform.AnchorMin = Vector2.One;
            guideText.RectTransform.AnchorMax = Vector2.One;
            guideText.RectTransform.Size = new Vector2(700, 100);
            guideText.RectTransform.AnchoredPosition = new Vector2(-300, -100);

            guideText.TextSize = 32;
            guideText.Color = Color.Black;
        }

        public override void OnUpdate()
        {
            if (Input.IsKeyPressed(KeyCode.W) || Input.IsKeyPressed(KeyCode.A)
                || Input.IsKeyPressed(KeyCode.S) || Input.IsKeyPressed(KeyCode.D))
            {
                Entity.Destroy();
            }
        }
    }
}