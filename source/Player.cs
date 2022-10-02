using System.Linq;
using Steel;
using SteelCustom.Skills;
using SteelCustom.UIComponents;

namespace SteelCustom
{
    public class Player : ScriptComponent
    {
        private RigidBody body;
        
        // Stats
        public float Speed = 0.3f;
        public int Health = 10;

        public int Score = 0;

        public Timeline Timeline { get; private set; }
        public Transformation CastPoint { get; private set; }

        public override void OnCreate()
        {
            SpriteRenderer sr = Entity.AddComponent<SpriteRenderer>();
            AsepriteData ad = ResourcesManager.LoadAsepriteData("player.aseprite");
            sr.Sprite = ad.Sprites.First();
            
            body = Entity.AddComponent<RigidBody>();
            body.RigidBodyType = RigidBodyType.Dynamic;
            body.GravityScale = 0.0f;
            body.FixedRotation = true;

            CircleCollider collider = Entity.AddComponent<CircleCollider>();
            collider.Radius = 0.2f;

            CastPoint = new Entity("Cast point", Entity).Transformation;
            CastPoint.LocalPosition = new Vector3(0.3f, 0.05f);
            
            Timeline = new Timeline();
            // TODO: for test
            //Timeline.InsertSkill(new Blast(), 2.0f);

            UI.CreateUIElement("Player UI", GameManager.InGameUIRoot).AddComponent<UIPlayerInfo>().Init(this);
            
            OnUpdate();
        }

        public override void OnUpdate()
        {
            Move();
            
            Timeline.Update(Time.DeltaTime);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            
            Entity.AddComponent<AudioSource>().Play(ResourcesManager.LoadAudioTrack("hurt.wav"));
            
            //Camera.Main.GetComponent<CameraController>().Shake(damage * 1.3f);

            if (Health <= 0)
            {
                Health = 0;
                Die();
            }
        }

        private void Die()
        {
            GameManager.Menu.OpenOnLoseScreen();
            Entity.Destroy();
        }

        private void Move()
        {
            Vector2 targetVelocity = Vector2.Zero;
            if (Input.IsKeyPressed(KeyCode.W))
                targetVelocity += new Vector2(0.0f, Speed);
            if (Input.IsKeyPressed(KeyCode.S))
                targetVelocity += new Vector2(0.0f, -Speed);
            if (Input.IsKeyPressed(KeyCode.D))
                targetVelocity += new Vector2(Speed, 0.0f);
            if (Input.IsKeyPressed(KeyCode.A))
                targetVelocity += new Vector2(-Speed, 0.0f);

            body.Velocity = targetVelocity.Normalize();
            
            Rotate(Camera.Main.ScreenToWorldPoint(Input.MousePosition));
        }
        
        private void Rotate(Vector3 target)
        {
            Vector2 vector = target - Transformation.Position;
            float angle = -Math.Atan2(vector.X, vector.Y) + Math.Pi * 0.27f;
            float current = Transformation.Rotation.Z;

            Transformation.Rotation = new Vector3(0.0f, 0.0f, Math.LerpAngle(current, angle, Time.DeltaTime * 20f));
        }
    }
}