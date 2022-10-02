using System.Collections.Generic;
using System.Linq;
using Steel;

namespace SteelCustom
{
    public class EnemyController : ScriptComponent
    {
        private float gameTimer = 0.0f;
        private float spawnTimer = 0.0f;
        private float spawnDelay = 5.0f;

        private List<float> levelChance = new List<float> { 1.0f, 0.0f, 0.0f, 0.0f };
        private List<float> levelMax = new List<float> { 1.0f, 3.0f, 3.0f, 1.0f };
        private List<float> levelStep = new List<float> { 0.0f, 0.025f, 0.005f, 0.001f };

        public override void OnCreate()
        {
            
        }

        public override void OnUpdate()
        {
            gameTimer += Time.DeltaTime;
            if (gameTimer >= 10.0f)
            {
                for (int i = 0; i < levelChance.Count; i++)
                    levelChance[i] = Math.Min(levelChance[i] + levelStep[i], levelMax[i]);
                gameTimer = 0.0f;
            }
            
            spawnTimer += Time.DeltaTime;
            if (spawnTimer >= spawnDelay)
            {
                spawnTimer = 0.0f;
                Spawn();
                spawnDelay = Math.Max(1.0f, spawnDelay * 0.985f);
            }
        }

        private int GetLevel()
        {
            float sum = levelChance.Sum();
            float seed = Random.NextFloat(0, sum);

            for (int i = 0; i < levelChance.Count; i++)
            {
                if (seed < levelChance[i])
                    return i + 1;

                seed -= levelChance[i];
            }

            return 1;
        }

        private void Spawn()
        {
            if (GameManager.Player == null || GameManager.Player.Entity.IsDestroyed())
                return;
            
            Enemy enemy = new Entity("Enemy", Entity).AddComponent<Enemy>();
            InitLevel(enemy, GetLevel());
            
            Vector3 playerPosition = GameManager.Player.Transformation.Position;
            Vector3 offset = GameManager.RandomPointOnCircle(2.0f);
            Vector3 position = playerPosition + offset;
            if (position.X > Map.Size || position.X < -Map.Size)
                offset = offset.SetX(-offset.X);
            if (position.Y > Map.Size || position.Y < -Map.Size)
                offset = offset.SetY(-offset.Y);
            
            enemy.Transformation.Position = playerPosition + offset;
        }

        private void InitLevel(Enemy enemy, int level)
        {
            switch (level)
            {
                case 1:
                    enemy.Init("goblin.aseprite", 0.2f, 0.25f, 1, 10);
                    break;
                case 2:
                    enemy.Init("goblin2.aseprite", 0.2f, 0.25f, 2, 20);
                    break;
                case 3:
                    enemy.Init("goblin3.aseprite", 0.2f, 0.25f, 3, 30);
                    break;
                case 4:
                    enemy.Init("ogre.aseprite", 0.35f, 0.1f, 10, 100);
                    break;
            }
        }
    }
}