using System.Collections.Generic;
using Steel;

namespace SteelCustom
{
    public class Map : ScriptComponent
    {
        public static float Size => (SIZE + 1) / 4;
        
        private const int SIZE = 20;

        public void Generate()
        {
            Sprite wall = ResourcesManager.LoadImage("wall.png");
            Sprite ground = ResourcesManager.LoadImage("ground.png");
            List<Sprite> groundDetails = new List<Sprite>();
            groundDetails.Add(ResourcesManager.LoadImage("ground1.png"));
            groundDetails.Add(ResourcesManager.LoadImage("ground2.png"));
            groundDetails.Add(ResourcesManager.LoadImage("ground3.png"));

            const float DETAIL_CHANCE = 0.1f;

            for (int i = -1; i < SIZE + 1; i++)
            {
                for (int j = -1; j < SIZE + 1; j++)
                {
                    if (i == -1 || j == -1 || i == SIZE || j == SIZE)
                    {
                        Entity entity = new Entity($"Wall {i} {j}", Entity);
                        entity.AddComponent<SpriteRenderer>().Sprite = wall;
                        entity.AddComponent<BoxCollider>();
                        entity.AddComponent<RigidBody>().RigidBodyType = RigidBodyType.Static;
                    
                        entity.Transformation.Position = new Vector3(
                            (i - (float)SIZE / 2) * 0.5f,
                            (j - (float)SIZE / 2) * 0.5f,
                            -1.0f
                        );
                    }
                    else
                    {
                        Entity entity = new Entity($"Tile {i} {j}", Entity);
                        entity.AddComponent<SpriteRenderer>().Sprite = 
                            Random.NextFloat(0.0f, 1.0f) >= DETAIL_CHANCE
                                ? ground
                                : groundDetails[Random.NextInt(0, groundDetails.Count - 1)];
                    
                        entity.Transformation.Position = new Vector3(
                            (i - (float)SIZE / 2) * 0.5f,
                            (j - (float)SIZE / 2) * 0.5f,
                            -1.0f
                        );
                    }
                }
            }
        }
    }
}