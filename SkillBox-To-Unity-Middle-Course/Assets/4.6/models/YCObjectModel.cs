using System;

namespace SkillBox.Course
{
    [Serializable]
    public struct YCObjectModel
    {
        public string heroName;
        public string heroClass;
        public float heroSpeed;
        public float heroDashDuration;
        public float heroDashSpeed;
        public float heroDashReloadDuration;
        public int heroAttackPower;

        public YCObjectModel(string heroName, string heroClass, float heroSpeed, float heroDashDuration, float heroDashSpeed, float heroDashReloadDuration, int heroAttackPower)
        {
            this.heroName = heroName;
            this.heroClass = heroClass;
            this.heroSpeed = heroSpeed;
            this.heroDashDuration = heroDashDuration;
            this.heroDashSpeed = heroDashSpeed;
            this.heroDashReloadDuration = heroDashReloadDuration;
            this.heroAttackPower = heroAttackPower;
        }
    }
}
