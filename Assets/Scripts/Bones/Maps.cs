using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts
{
    public static class Mapper
    {
        public static readonly Dictionary<Body25, HumanBodyBones> Body25Map = new Dictionary<Body25, HumanBodyBones>()
        {
            {Body25.LAnkle, HumanBodyBones.LeftFoot},
            {Body25.Nose, HumanBodyBones.Jaw} // Don't ask why... I just try to compare this shit
        };
    }
}
