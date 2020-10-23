
       
using System;
using System.Collections.Generic;

namespace HitPointAPI
{
        public class HPChange{
            public string changeType { get; set; }
            public string changeMode { get; set; }
            public int changeValue { get; set; }
            public int changeModifier { get; set; }
            public string damageType {get; set;}
        }
}