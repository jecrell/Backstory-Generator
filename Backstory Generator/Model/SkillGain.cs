using System.Collections;

namespace Backstory_Generator
{
    public class SkillGain : IEqualityComparer
    {
        public SkillDef defName { get; set; }
        public int amount { get; set; }

        public new bool Equals(object x, object y)
        {
            if (x is SkillGain sx &&
                y is SkillGain sy &&
                sx.defName == sy.defName)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(object obj)
        {
            return (defName + amount).GetHashCode();
        }
    }
}
