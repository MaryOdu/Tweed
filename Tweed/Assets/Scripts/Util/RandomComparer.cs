using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Util
{
    internal class RandomComparer : IComparer<GameObject>
    {

        public RandomComparer()
        {
        }

        public int Compare(GameObject x, GameObject y)
        {
            return UnityEngine.Random.Range(0, 100);
        }
    }
}
