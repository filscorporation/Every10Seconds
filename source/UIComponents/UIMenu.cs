using Steel;

namespace SteelCustom.UIComponents
{
    public class UIMenu : ScriptComponent
    {
        private bool menuOpened;

        private Entity menu;
        private Entity lostMenu;
        private UIButton playButton;
        private UIButton restartButton;
        private UIButton continueButton;
        private UIButton soundButton;
        private bool lostMenuOpened = false;
        private float savedTimeScale = 1.0f;

        private Color textColor = new Color(240, 233, 201);

        public override void OnCreate()
        {
            GetComponent<RectTransformation>().AnchorMin = Vector2.Zero;
            GetComponent<RectTransformation>().AnchorMax = Vector2.One;

            menu = UI.CreateUIImage(ResourcesManager.LoadImage("blur.png"), "Menu", Entity).Entity;
            menu.GetComponent<UIImage>().Color = new Color(0.0f, 0.0f, 0.0f, 0.45f);
            RectTransformation menuRT = menu.GetComponent<RectTransformation>();
            menuRT.AnchorMin = Vector2.Zero;
            menuRT.AnchorMax = Vector2.One;

            playButton = CreateMenuButton("Play", 120);
            playButton.OnClick.AddCallback(Play);
            continueButton = CreateMenuButton("Continue", 120);
            continueButton.OnClick.AddCallback(CloseMenu);
            continueButton.Entity.IsActiveSelf = false;
            restartButton = CreateMenuButton("Restart", 240);
            restartButton.OnClick.AddCallback(Restart);
            restartButton.Entity.IsActiveSelf = false;
            CreateMenuButton("Exit", 0).OnClick.AddCallback(Exit);
            soundButton = CreateSoundButton();
            CreateAbout();
        }

        public override void OnUpdate()
        {
            if (Input.IsKeyJustPressed(KeyCode.Escape))
            {
                if (menuOpened)
                    CloseMenu();
                else if (GameManager.Player != null && !GameManager.Player.Entity.IsDestroyed())
                {
                    if (GameManager.SkillSpawner.IsPlacingSkill || GameManager.SkillSpawner.IsRemovingSkill)
                        GameManager.SkillSpawner.Cancel();
                    else
                        OpenMenu();
                }
            }
        }

        public void OpenOnLoseScreen()
        {
            lostMenuOpened = true;
            
            menu.IsActiveSelf = false;
            
            lostMenu = UI.CreateUIImage(ResourcesManager.LoadImage("blur.png"), "Menu", Entity).Entity;
            lostMenu.GetComponent<UIImage>().Color = new Color(0.0f, 0.0f, 0.0f, 0.45f);
            RectTransformation menuRT = lostMenu.GetComponent<RectTransformation>();
            menuRT.AnchorMin = Vector2.Zero;
            menuRT.AnchorMax = Vector2.One;

            {
                UIText uiText = UI.CreateUIText("You lost!", "Text", lostMenu);
                uiText.Color = textColor;
                uiText.TextSize = 64;
                uiText.TextAlignment = AlignmentType.CenterMiddle;

                uiText.RectTransform.AnchorMin = new Vector2(0.5f, 0.5f);
                uiText.RectTransform.AnchorMax = new Vector2(0.5f, 0.5f);
                uiText.RectTransform.Size = new Vector2(400, 100);
                uiText.RectTransform.AnchoredPosition = new Vector2(0, 300);
            }
            {
                UIText uiText = UI.CreateUIText($"Your score is {GameManager.Player.Score}!", "Text", lostMenu);
                uiText.Color = textColor;
                uiText.TextSize = 32;
                uiText.TextAlignment = AlignmentType.CenterLeft;
                uiText.TextOverflowMode = OverflowMode.WrapByWords;

                uiText.RectTransform.AnchorMin = new Vector2(0.5f, 0.5f);
                uiText.RectTransform.AnchorMax = new Vector2(0.5f, 0.5f);
                uiText.RectTransform.Size = new Vector2(400, 200);
                uiText.RectTransform.AnchoredPosition = new Vector2(0, 160);
            }

            CreateLostMenuButton("Restart", 0).OnClick.AddCallback(Restart);
            CreateLostMenuButton("Exit", 120).OnClick.AddCallback(Exit);
            CreateAbout();
        }

        private void OpenMenu()
        {
            if (lostMenuOpened)
                return;
            
            savedTimeScale = Time.TimeScale;
            Time.TimeScale = 0.0f;
            
            menuOpened = true;
            menu.IsActiveSelf = true;
        }

        private void CloseMenu()
        {
            if (lostMenuOpened)
                return;
            
            Time.TimeScale = savedTimeScale;
            Log.LogInfo("Time scale " + savedTimeScale);
            
            menuOpened = false;
            menu.IsActiveSelf = false;
        }

        private void Play()
        {
            continueButton.Entity.IsActiveSelf = true;
            restartButton.Entity.IsActiveSelf = true;
            playButton.Entity.IsActiveSelf = true;

            CloseMenu();
            
            GameManager.StartGame();
        }

        private void Restart()
        {
            lostMenu?.Destroy();
            lostMenuOpened = false;
            
            continueButton.Entity.IsActiveSelf = true;
            restartButton.Entity.IsActiveSelf = true;
            playButton.Entity.IsActiveSelf = true;

            CloseMenu();
            
            GameManager.RestartGame();
        }

        private void Exit()
        {
            Application.Quit();
        }

        private void ChangeSound()
        {
            if (GameManager.SoundOn)
            {
                GameManager.SoundOn = false;
                Camera.Main.Entity.AddComponent<AudioListener>().Volume = 0.0f;
                soundButton.TargetImage.Sprite = ResourcesManager.LoadImage("ui_sound_off.png");
            }
            else
            {
                GameManager.SoundOn = true;
                Camera.Main.Entity.AddComponent<AudioListener>().Volume = GameManager.DEFAULT_VOLUME;
                soundButton.TargetImage.Sprite = ResourcesManager.LoadImage("ui_sound_on.png");
            }
        }

        private UIButton CreateMenuButton(string text, float y)
        {
            Sprite sprite = ResourcesManager.LoadImage("ui_frame.png");
            sprite.SetAs9Sliced(3);
            sprite.PixelsPerUnit = 64;
            UIButton button = UI.CreateUIButton(sprite, "Menu button", menu);
            button.RectTransform.AnchorMin = new Vector2(0.65f, 0.35f);
            button.RectTransform.AnchorMax = new Vector2(0.65f, 0.35f);
            button.RectTransform.Size = new Vector2(200, 100);
            button.RectTransform.AnchoredPosition = new Vector2(0, y);

            UIText uiText = UI.CreateUIText(text, "Label", button.Entity);
            uiText.Color = textColor;
            uiText.TextSize = 32;
            uiText.TextAlignment = AlignmentType.CenterMiddle;
            uiText.RectTransform.AnchorMin = Vector2.Zero;
            uiText.RectTransform.AnchorMax = Vector2.One;

            return button;
        }

        private UIButton CreateLostMenuButton(string text, float y)
        {
            Sprite sprite = ResourcesManager.LoadImage("ui_frame.png");
            sprite.SetAs9Sliced(3);
            sprite.PixelsPerUnit = 64;
            UIButton button = UI.CreateUIButton(sprite, "Menu button", lostMenu);
            button.RectTransform.AnchorMin = new Vector2(0.65f, 0.35f);
            button.RectTransform.AnchorMax = new Vector2(0.65f, 0.35f);
            button.RectTransform.Size = new Vector2(200, 100);
            button.RectTransform.AnchoredPosition = new Vector2(0, y);

            UIText uiText = UI.CreateUIText(text, "Label", button.Entity);
            uiText.Color = textColor;
            uiText.TextSize = 32;
            uiText.TextAlignment = AlignmentType.CenterMiddle;
            uiText.RectTransform.AnchorMin = Vector2.Zero;
            uiText.RectTransform.AnchorMax = Vector2.One;

            return button;
        }

        private UIButton CreateSoundButton()
        {
            Sprite sprite = ResourcesManager.LoadImage("ui_sound_on.png");
            sprite.PixelsPerUnit = 64;
            UIButton button = UI.CreateUIButton(sprite, "Sound button", menu);
            button.RectTransform.AnchorMin = new Vector2(0.65f, 0.35f);
            button.RectTransform.AnchorMax = new Vector2(0.65f, 0.35f);
            button.RectTransform.Size = new Vector2(64, 64);
            button.RectTransform.AnchoredPosition = new Vector2(170, -120);
            
            button.OnClick.AddCallback(ChangeSound);

            return button;
        }

        private void CreateAbout()
        {
            UIText text = UI.CreateUIText("Created in 48 hours for LD51 using Steel Engine", "About", menu);
            text.Color = textColor;
            text.TextSize = 32;
            text.RectTransform.AnchorMin = new Vector2(0.5f, 0.0f);
            text.RectTransform.AnchorMax = new Vector2(1.0f, 0.0f);
            text.RectTransform.Pivot = new Vector2(0.0f, 0.0f);
            text.RectTransform.Size = new Vector2(0, 40);
        }
    }
}