using Steel;
using SteelCustom.UIComponents;

namespace SteelCustom
{
    public class GameManager
    {
        public static UIMenu Menu;
        public static Entity InGameUIRoot;
        
        public static Map Map;
        public static Player Player;
        public static EnemyController EnemyController;
        public static SkillSpawner SkillSpawner;
        public static bool SoundOn = true;
        
        public const float DEFAULT_VOLUME = 0.15f;

        public static void EntryPoint()
        {
            InGameUIRoot = UI.CreateUIElement("UI root");
            InGameUIRoot.GetComponent<RectTransformation>().AnchorMin = Vector2.Zero;
            InGameUIRoot.GetComponent<RectTransformation>().AnchorMax = Vector2.One;
            
            Time.FixedDeltaTime = 1.0f / 240.0f;
            Screen.Color = new Color(194, 181, 169);
            Screen.Width = 1366;
            Screen.Height = 768;

            Camera.Main.Height = 4f;
            
            Camera.Main.Entity.AddComponent<CameraController>();
            Camera.Main.Entity.GetComponent<AudioListener>().Volume = DEFAULT_VOLUME;
            
            Map = new Entity("Map").AddComponent<Map>();
            Map.Generate();

            Menu = UI.CreateUIElement("Menu").AddComponent<UIMenu>();

            Entity backgroundMusic = new Entity();
            AudioSource source = backgroundMusic.AddComponent<AudioSource>();
            source.Loop = true;
            source.Play(ResourcesManager.LoadAudioTrack("background_music.wav"));
            source.Volume = 0.1f;
        }

        public static void StartGame()
        {
            //Time.TimeScale = 5;
            
            Camera.Main.Transformation.Position = new Vector3(0.0f, 0.0f, -3.0f);

            Map?.Entity.Destroy();
            Map = new Entity("Map").AddComponent<Map>();
            Map.Generate();
            
            Player = new Entity("Player").AddComponent<Player>();
            
            EnemyController = new Entity("Enemy controller").AddComponent<EnemyController>();
            
            SkillSpawner = new Entity("Skill spawner").AddComponent<SkillSpawner>();

            UI.CreateUIElement("UI guide", InGameUIRoot).AddComponent<UIGuide>();
        }

        public static void RestartGame()
        {
            InGameUIRoot.Destroy();
            InGameUIRoot = UI.CreateUIElement("UI root");
            InGameUIRoot.GetComponent<RectTransformation>().AnchorMin = Vector2.Zero;
            InGameUIRoot.GetComponent<RectTransformation>().AnchorMax = Vector2.One;
            
            Map.Entity.Destroy();
            Map = null;
            
            Player.Entity.Destroy();
            Player = null;

            EnemyController.Entity.Destroy();
            EnemyController = null;

            SkillSpawner.Entity.Destroy();
            SkillSpawner = null;
            
            StartGame();
        }

        public static Vector3 RandomPointOnCircle(float radius)
        {
            float angle = Random.NextFloat(0, 2 * Math.Pi);
            return new Vector3(Math.Cos(angle) * radius, Math.Sin(angle) * radius);
        }
    }
}