using System.Collections.Generic;
using UnityEngine;

namespace Boy
{
    public class Utility : MonoBehaviour
    {
        public static void ShuffleChildren(Transform parent)
        {
            List<Transform> children = new List<Transform>();

            foreach (Transform c in parent)
                children.Add(c);

            for (int i = children.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var tmp = children[i];
                children[i] = children[j];
                children[j] = tmp;
            }

            for (int i = 0; i < children.Count; i++)
                children[i].SetSiblingIndex(i);
        }

    }
}
