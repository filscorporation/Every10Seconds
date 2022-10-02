using System;
using System.Collections.Generic;
using System.Linq;
using Steel;
using SteelCustom.Skills;
using SteelCustom.UIComponents;
using Math = Steel.Math;

namespace SteelCustom
{
    public class Timeline
    {
        public struct TimelineSkill
        {
            public Skill Skill;
            public float Position;
        }

        public UITimeline UITimeline { get; }

        public int BlastsLearned = 0;
        public float Progress { get; private set; }
        public List<TimelineSkill> Skills { get; private set; } = new List<TimelineSkill>();

        private Skill currentSkill;
        private float deactivateCurrentSkill = 0;
        private int nextSkill = 0;

        public Timeline()
        {
            UITimeline = UI.CreateUIElement("UI timeline", GameManager.InGameUIRoot).AddComponent<UITimeline>();
            RectTransformation rt = UITimeline.Entity.GetComponent<RectTransformation>();
            rt.AnchorMin = Vector2.Zero;
            rt.AnchorMax = Vector2.One;
        }

        public void Update(float deltaTime)
        {
            Progress += deltaTime;
            if (currentSkill != null && Progress >= deactivateCurrentSkill)
            {
                UITimeline.DeactivateSkill(currentSkill);
                
                currentSkill.EndUse();
                currentSkill = null;
            }
            if (Progress >= 10.0f)
            {
                Progress = 0.0f;
                nextSkill = 0;
            }
            UITimeline.SetProgress(Progress);

            if (Skills.Count > nextSkill && Skills[nextSkill].Position <= Progress)
            {
                currentSkill = Skills[nextSkill].Skill;
                currentSkill.Use();
                deactivateCurrentSkill = Progress + currentSkill.Duration;
                
                UITimeline.ActivateSkill(currentSkill);
                
                nextSkill++;
            }
        }

        public void InsertSkill(Skill skill, float position)
        {
            TimelineSkill newElement = new TimelineSkill { Skill = skill, Position = position };
            Skills.Add(newElement);
            if (skill is Blast)
                BlastsLearned++;
            Skills = Skills.OrderBy(s => s.Position).ToList();
            int newIndex = Skills.IndexOf(newElement);
            if (newIndex == nextSkill)
            {
                if (position <= Progress)
                    nextSkill++;
            }
            else if (newIndex < nextSkill)
                nextSkill++;
            
            UITimeline.AddSkill(skill, position);
        }

        public void RemoveSkill(Skill skill)
        {
            int i = Skills.FindIndex(s => ReferenceEquals(s.Skill, skill));
            if (i < 0)
                return;
            
            Skills.RemoveAt(i);
            if (i <= nextSkill)
                nextSkill--;

            if (nextSkill < 0)
                nextSkill = Skills.Count - 1;
            
            UITimeline.RemoveSkill(skill);
        }

        public bool CanInsert(Skill skill, float position, out float betterPosition)
        {
            position = Math.Clamp(position, 0.0f, 10.0f);
            
            int left = -1, right = -1;
            for (int i = 0; i < Skills.Count; i++)
            {
                if (Skills[i].Position >= position)
                {
                    right = i;
                    break;
                }

                left = i;
            }

            betterPosition = position;

            if (left == -1)
            {
                if (right == -1)
                    return true; // No other skills

                if (position + skill.Duration <= Skills[right].Position)
                    return true; // Can fit on the current position left to next skill
                if (Skills[right].Position - skill.Duration >= 0.0f)
                {
                    betterPosition = Skills[right].Position - skill.Duration;
                    return true; // Move to fit just before next skill
                }
                
                return false; // No space to the left of next skill
            }

            if (right == -1)
            {
                if (Skills[left].Position + Skills[left].Skill.Duration <= position)
                {
                    if (position + skill.Duration <= 10.0f)
                        return true; // Can fit on the current position right to previous skill
                    
                    if (Skills[left].Position + Skills[left].Skill.Duration + skill.Duration <= 10.0f)
                    {
                        betterPosition = 10.0f - skill.Duration;
                        return true; // Move to fit before end
                    }
                    
                    return false; // No space to the right of previous skill
                }
                
                if (Skills[left].Position + Skills[left].Skill.Duration + skill.Duration <= 10.0f)
                {
                    betterPosition = Skills[left].Position + Skills[left].Skill.Duration;
                    return true; // Move to fit right after previous skill
                }

                return false; // No space to the right of previous skill
            }

            if (Skills[left].Position + Skills[left].Skill.Duration <= position
                && position <= Skills[right].Position)
            {
                if (position + skill.Duration <= Skills[right].Position)
                    return true; // Can fit between neighbour skills

                if (Skills[left].Position + Skills[left].Skill.Duration + skill.Duration <= Skills[right].Position)
                {
                    betterPosition = Skills[right].Position - skill.Duration;
                    return true; // Move to fit just before next skill
                }

                return false; // No space between neighbour skills
            }

            if (Skills[left].Position + Skills[left].Skill.Duration > position)
            {
                if (Skills[left].Position + Skills[left].Skill.Duration + skill.Duration <= Skills[right].Position)
                {
                    betterPosition = Skills[left].Position + Skills[left].Skill.Duration;
                    return true; // Move to fit right after previous skill
                }

                return false; // No space between neighbour skills
            }
            
            if (Skills[left].Position + Skills[left].Skill.Duration <= Skills[right].Position - skill.Duration)
            {
                betterPosition = Skills[right].Position - skill.Duration;
                return true; // Move to fit right before next skill
            }
            
            return false; // No space between neighbour skills
        }
    }
}