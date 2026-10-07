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
    }
}
