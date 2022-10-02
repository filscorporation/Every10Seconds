using System;
using System.Collections;
using System.Linq;
using Steel;
using Math = Steel.Math;

namespace SteelCustom
{
    public class Enemy : ScriptComponent
    {
        public RigidBody Body;
        
        private float speed = 0.25f;
        private float size;
        private int maxHealth = 1;
        private int health = 1;
        private int score = 10;

        private float freezeDuration = 0.0f;
        public bool IsFrozen = false;
        private Entity freezeEffect = null;

        private float attackTimer;
        private const float ATTACK_DELAY = 1.0f;

        private Animator animator;

        public override void OnCreate()
        {
            Entity.AddComponent<SpriteRenderer>();
            
            Body = Entity.AddComponent<RigidBody>();
            Body.RigidBodyType = RigidBodyType.Dynamic;
            Body.GravityScale = 0.0f;
            Body.FixedRotation = true;

            Entity.AddComponent<CircleCollider>();
        }
        
        public override void OnUpdate()
        {
            freezeDuration -= Time.DeltaTime;
            if (IsFrozen && freezeDuration <= 0.0f)
            {
                IsFrozen = false;
                freezeEffect?.Destroy();
                freezeEffect = null;
            }
            
            if (IsFrozen)
            {
                Body.Velocity = Math.Lerp(Body.Velocity, Vector2.Zero, Time.DeltaTime);
                return;
            }
            
            attackTimer -= Time.DeltaTime;
            
            if (GameManager.Player == null || GameManager.Player.Entity.IsDestroyed())
                return;

            if (Vector3.Distance(Transformation.Position, GameManager.Player.Transformation.Position) < size + 0.25f)
                StartCoroutine(Attack());
            else
                Move();
        }

        public void Init(string spritePath, float size, float speed, int health, int score)
        {
            AsepriteData ad = ResourcesManager.LoadAsepriteData(spritePath);

            Entity.GetComponent<SpriteRenderer>().Sprite = ad.Sprites.First();
            
            animator = Entity.AddComponent<Animator>();
            animator.AddAnimations(ad.Animations);
            animator.Play("Idle");

            this.size = size;
            Entity.GetComponent<CircleCollider>().Radius = size;
            Entity.GetComponent<RigidBody>().Mass = size * 5;
            
            this.speed = speed;
            maxHealth = health;
            this.health = health;
            this.score = score;
        }

        private void Move()
        {
            Vector2 targetVelocity = Vector2.Zero;
            Transformation target = GameManager.Player.Transformation;
            
            targetVelocity = (target.Transformation.Position - Transformation.Position).Normalize() * speed;

            Body.Velocity = Math.Lerp(Body.Velocity, targetVelocity, Time.DeltaTime * 5.0f);
            
            Rotate(target.Position);
        }
        
        private void Rotate(Vector3 target)
        {
            Vector2 vector = target - Transformation.Position;
            float angle = -Math.Atan2(vector.X, vector.Y) + Math.Pi / 4;
            float current = Transformation.Rotation.Z;

            Transformation.Rotation = new Vector3(0.0f, 0.0f, Math.LerpAngle(current, angle, Time.DeltaTime * 15f));
        }

        private IEnumerator Attack()
        {
            if (attackTimer <= 0.0f)
            {
                attackTimer = ATTACK_DELAY;
                animator.Play("Attack");
                
                GameManager.Player.TakeDamage(1);
            }

            yield return new WaitForSeconds(0.35f);
            animator.Play("Idle");
        }

        public void Freeze(float duration)
        {
            if (!IsFrozen)
            {
                freezeEffect = new Entity("Freeze effect", Entity);
                freezeEffect.AddComponent<SpriteRenderer>().Sprite = ResourcesManager.LoadImage("freeze_effect.png");
                freezeEffect.Transformation.LocalPosition = new Vector3(0.0f, 0.0f, 0.2f);
            }
            
            IsFrozen = true;

            if (freezeDuration < 0.0)
                freezeDuration = 0.0f;
            freezeDuration += duration;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;

            if (health <= 0)
            {
                health = 0;
                
                Die();
            }
        }

        public void Die()
        {
            GameManager.Player.Score += score;
            Entity.Destroy();
                
            Entity effect = ResourcesManager.LoadAsepriteData("blood_splash.aseprite").CreateEntityFromAsepriteData();
            effect.Transformation.Position = Transformation.Position + new Vector3(0.0f, 0.0f, 0.5f);
            effect.GetComponent<Animator>().Play("Effect");
            effect.Destroy(0.7f);
        }
    }
}