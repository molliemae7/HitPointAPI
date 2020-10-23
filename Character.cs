
using System;
using System.Collections.Generic;

namespace HitPointAPI
{
     public class Character
        {
            public string name { get; set; }
            public int level { get; set; }
            public List<Classes> classes { get; set; }
            public Stats stats { get; set; }
            public List<Items> items { get; set; }
            public List<Defenses> defenses {get; set;}
            public HPValues hpValues {get; set;}
        }
        public class Classes{
            public string name { get; set; }
            public int hitDiceValue { get; set; }
            public int classLevel { get; set; }
        }
        public class Stats{
            public int strength { get; set; }
            public int dexterity { get; set; }
            public int constitution { get; set; }
            public int intelligence { get; set; }
            public int wisdom { get; set; }
            public int charisma { get; set; }
        }
        public class Items{
            public string name { get; set; }
            public Modifier modifier { get; set; }
        }
        public class Modifier{

            public string affectedObject { get; set; }
            public string affectedValue { get; set; }
            public int value { get; set; }
        }
        public class Defenses{
            public string type { get; set; }
            public string defense { get; set; }
        }
}