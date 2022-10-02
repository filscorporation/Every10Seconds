using Steel;

namespace SteelCustom.UIComponents
{
    public class UIPlayerInfo : ScriptComponent
    {
        private UIText healthText;
        private UIText scoreText;
        
        public void Init(Player player)
        {
            RectTransformation rt = Entity.GetComponent<RectTransformation>();
            rt.AnchorMin = Vector2.Zero;
            rt.AnchorMax = Vector2.One;
            
            UIImage healthImage = UI.CreateUIImage(ResourcesManager.LoadImage("ui_health.png"), "UI health", Entity);
            healthImage.RectTransform.AnchorMin = new Vector2(0.0f, 1.0f);
            healthImage.RectTransform.AnchorMax = new Vector2(0.0f, 1.0f);
            healthImage.RectTransform.Pivot = new Vector2(0.0f, 1.0f);
            healthImage.RectTransform.AnchoredPosition = new Vector2(10, 0);
            healthImage.RectTransform.Size = new Vector2(100, 100);

            healthText = UI.CreateUIText(player.Health.ToString(), "UI health", Entity);
            healthText.RectTransform.AnchorMin = new Vector2(0.0f, 1.0f);
            healthText.RectTransform.AnchorMax = new Vector2(0.0f, 1.0f);
            healthText.RectTransform.Pivot = new Vector2(0.0f, 1.0f);
            healthText.RectTransform.AnchoredPosition = new Vector2(110, 0);
            healthText.RectTransform.Size = new Vector2(100, 100);

            healthText.TextSize = 64;
            healthText.Color = Color.Black;
            healthText.TextAlignment = AlignmentType.CenterLeft;
            
            UIImage scoreImage = UI.CreateUIImage(ResourcesManager.LoadImage("ui_star.png"), "UI star", Entity);
            scoreImage.RectTransform.AnchorMin = new Vector2(1.0f, 1.0f);
            scoreImage.RectTransform.AnchorMax = new Vector2(1.0f, 1.0f);
            scoreImage.RectTransform.Pivot = new Vector2(1.0f, 1.0f);
            scoreImage.RectTransform.AnchoredPosition = new Vector2(-10, 0);
            scoreImage.RectTransform.Size = new Vector2(100, 100);

            scoreText = UI.CreateUIText(player.Score.ToString(), "UI score", Entity);
            scoreText.RectTransform.AnchorMin = new Vector2(1.0f, 1.0f);
            scoreText.RectTransform.AnchorMax = new Vector2(1.0f, 1.0f);
            scoreText.RectTransform.Pivot = new Vector2(1.0f, 1.0f);
            scoreText.RectTransform.AnchoredPosition = new Vector2(-110, 0);
            scoreText.RectTransform.Size = new Vector2(400, 100);

            scoreText.TextSize = 64;
            scoreText.Color = Color.Black;
            scoreText.TextAlignment = AlignmentType.CenterRight;
        }

        public override void OnUpdate()
        {
            if (GameManager.Player == null || GameManager.Player.Entity.IsDestroyed())
                return;
            
            healthText.Text = GameManager.Player.Health.ToString();
            scoreText.Text = GameManager.Player.Score.ToString();
        }
    }
}