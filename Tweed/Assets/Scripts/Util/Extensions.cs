using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;

using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class Extensions
    {
        /// <summary>
        /// Queries a gameobjects parents and will stop and return true at the first parent where the provided function returns evaluates to true.
        /// </summary>
        /// <param name="gameObject">GameObject to evaluate</param>
        /// <param name="func">Eval, function</param>
        /// <returns>Is found? (true/false)</returns>
        public static bool QueryParents(this GameObject gameObject, Func<GameObject, bool> func)
        {
            if (func(gameObject))
            {
                return true;
            }

            if (gameObject.transform.parent != null)
            {
                return QueryParents(gameObject.transform.parent.gameObject, func);
            }

            return false;
        }

        /// <summary>
        /// Recurses back up through the heirarch in search of a gameobjects parent whose name matches that which has been provided.
        /// </summary>
        /// <param name="gameObject">The gameobject to evaluate</param>
        /// <param name="name">The name of the gameobject to find.</param>
        /// <returns>Found GameObject</returns>
        public static GameObject FindParent(this GameObject gameObject, string name)
        {
            if (gameObject.name == name)
            {
                return gameObject;
            }

            while (gameObject.transform.parent != null)
            {
                var parent = gameObject.transform.parent.gameObject;

                if (parent != null)
                {
                    FindParent(parent, name);
                }
            }

            return null;
        }


        /// <summary>
        /// This search is likely to be slow - so only use it once at initialisation. Not at runtime.
        /// Performs a depth-first search for any child object that has a mataching name.
        /// </summary>
        /// <param name="gameObject">Starting GameObject</param>
        /// <param name="name">Name to search thought heirarchy for.</param>
        /// <returns>Found gameobject. Null if not found.</returns>
        public static GameObject FindChild(this GameObject gameObject, string name)
        {
            if (name.Contains('.'))
            {
                return FindChild_ByPath(gameObject, name);
            }

            return Find_By_DepthFirstSearch(gameObject, name);
        }

        /// <summary>
        /// Performs a depth first search in the gameObject heiarchy for a child with a matching name.
        /// </summary>
        /// <param name="gameObject">Starting gameObject</param>
        /// <param name="name">Name to be queried</param>
        /// <returns>Found GameObject. Null, if not found.</returns>
        private static GameObject Find_By_DepthFirstSearch(GameObject gameObject, string name)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var child = gameObject.transform.GetChild(i);

                if (child.name == name)
                {
                    return child.gameObject;
                }

                var obj = Find_By_DepthFirstSearch(child.gameObject, name);

                if (obj != null)
                    return obj;
            }

            return null;
        }

        /// <summary>
        /// Searches through the gameObject heirarchy on the path provided. Allows of explicit searching through the tree.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static GameObject FindChild_ByPath(GameObject gameObject, string name)
        {
            var tokens = name.Split('.');

            var currObj = gameObject.transform.Find(tokens[0])?.gameObject;

            if (tokens.Length == 1)
            {
                return currObj;
            }

            var newPath = string.Join(".", tokens.Skip(1));

            return FindChild_ByPath(currObj, newPath);
        }
    }
}
