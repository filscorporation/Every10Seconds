using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steel;
using SteelCustom.Skills;
using SteelCustom.UIComponents;

namespace SteelCustom
{
    public class SkillSpawner : ScriptComponent
    {
        public bool IsPlacingSkill;
        public bool IsRemovingSkill;
        
        private Skill currentSkillToInsert;
        public Skill CurrentSkillToRemove;

        private float gameTimer = 0.0f;
        private float spawnTimer = 0.0f;
        private float spawnDelay = 10.0f;

        private List<Skill> skills = new List<Skill>();
        
        private bool stop = false;
        private Entity tooltip;

        public override void OnCreate()
        {
            skills.Add(new Amnesia());
            skills.Add(new Blackhole());
            skills.Add(new Blast());
            skills.Add(new BlastWave());
            skills.Add(new Earthquake());
            skills.Add(new Fireball());
            skills.Add(new Frostbolt());
            skills.Add(new Shockwave());
            
            SpawnSkillObject(new Blast());
        }

        public override void OnUpdate()
        {
            Vector3 position = Camera.Main.ScreenToWorldPoint(Input.MousePosition);
            position = position.SetZ(0.0f);
            foreach (SkillObject skillObject in FindAllOfType<SkillObject>())
            {
                if (Vector2.Distance(skillObject.Transformation.Position, position) < 0.5f)
                {
                    tooltip = UITooltip.ShowTooltip(skillObject.Skill);
                    break;
                }
            }
            
            if (GameManager.Player == null || GameManager.Player.Entity.IsDestroyed())
                return;
            
            gameTimer += Time.DeltaTime;
            spawnTimer += Time.DeltaTime;
            if (gameTimer > 20.0f)
                spawnDelay = 5.0f;
            if (spawnTimer >= spawnDelay)
            {
                spawnTimer = 0.0f;
                SpawnRandomSkillObject();
            }

            if (currentSkillToInsert != null)
            {
                if (Input.IsMouseJustPressed(MouseCodes.ButtonLeft))
                {
                    Timeline t = GameManager.Player.Timeline;
                    if (t.CanInsert(currentSkillToInsert, t.UITimeline.MousePosition, out float betterPosition))
                        t.InsertSkill(currentSkillToInsert, betterPosition);

                    GameManager.Player.Timeline.UITimeline.DeactivateInsertSkillUI();
                
                    currentSkillToInsert = null;
                    Time.TimeScale = 1.0f;
                    IsPlacingSkill = false;
                }
            }

            if (IsRemovingSkill && CurrentSkillToRemove != null)
            {
                if (Input.IsMouseJustPressed(MouseCodes.ButtonLeft))
                {
                    GameManager.Player.Timeline.RemoveSkill(CurrentSkillToRemove);

                    GameManager.Player.Timeline.UITimeline.DeactivateInsertSkillUI();

                    Time.TimeScale = 1.0f;
                    IsRemovingSkill = false;
                    CurrentSkillToRemove = null;
                }
            }
        }

        public void Cancel()
        {
            if (IsRemovingSkill)
            {
                IsRemovingSkill = false;
                Time.TimeScale = 1.0f;
                GameManager.Player.Timeline.UITimeline.DeactivateInsertSkillUI();
                
                return;
            }
            
            if (currentSkillToInsert == null)
                return;
            
            currentSkillToInsert = null;
            Time.TimeScale = 1.0f;
            IsPlacingSkill = false;
            GameManager.Player.Timeline.UITimeline.DeactivateInsertSkillUI();
        }

        private void SpawnRandomSkillObject()
        {
            SpawnSkillObject(RandomWeightedSkill());
        }
        
        private Skill RandomWeightedSkill()
        {
            float sum = skills.Sum(s => s.MinTimeToSpawn >= gameTimer ? 0.0f : s.Rarity);
            float seed = Random.NextFloat(0, sum);

            Skill selectedSkill = null;
            foreach (Skill skill in skills.Where(s => s.MinTimeToSpawn < gameTimer))
            {
                if (seed < skill.Rarity)
                {
                    selectedSkill = skill;
                    break;
                }

                seed -= skill.Rarity;
            }

            return selectedSkill;
        }
        
        private void SpawnSkillObject(Skill skill)
        {
            SkillObject so = new Entity("Skill object", Entity).AddComponent<SkillObject>();

            float x = Map.Size * 0.7f;
            Vector3 position = new Vector3(Random.NextFloat(-x, x), Random.NextFloat(-x, x));
            if (Vector3.Distance(position, GameManager.Player.Transformation.Position) < 1.5f)
                position = GameManager.Player.Transformation.Position + GameManager.RandomPointOnCircle(1.5f);
            
            so.Transformation.Position = position + new Vector3(0.0f, 0.0f, -0.5f);
            
            so.Init(skill);
        }
        
        public void TryInsertSkill(Skill skill)
        {
            StartCoroutine(TryInsertSkillCoroutine(skill));
        }

        private IEnumerator SlowTime()
        {
            float timer = 1.0f;
            while (!stop)
            {
                Time.TimeScale = timer;
                timer -= Time.DeltaTime * 1.5f;
                
                yield return null;
            }
        }
        
        private IEnumerator TryInsertSkillCoroutine(Skill skill)
        {
            if (skill is Amnesia)
                IsRemovingSkill = true;
            else
                IsPlacingSkill = true;
            
            stop = false;
            StartCoroutine(SlowTime());
            
            yield return new WaitForSecondsUnscaled(1.0f);
            
            Time.TimeScale = 0.0f;

            if (skill is Amnesia)
            {
                GameManager.Player.Timeline.UITimeline.ActivateRemoveSkillUI(skill);
            }
            else
            {
                currentSkillToInsert = skill;
                GameManager.Player.Timeline.UITimeline.ActivateInsertSkillUI(skill);
            }

            stop = true;
        }
    }
}