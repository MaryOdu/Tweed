using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public class GameObjectPositionComparer : IComparer<GameObject>
    {
        private Vector3Comparer m_v3Comparer;

        public GameObjectPositionComparer()
        {
            m_v3Comparer = new Vector3Comparer();
        }

        public int Compare(GameObject x, GameObject y)
        {
            var v1 = x.transform.position;
            var v2 = y.transform.position;

            return m_v3Comparer.Compare(v1, v2);
        }
    }

}
