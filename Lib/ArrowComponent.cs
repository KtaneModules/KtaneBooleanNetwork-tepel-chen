using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BooleanNetwork
{
    class ArrowComponent: MonoBehaviour
    {
        [SerializeField]
        internal Mesh ArrowLongT;
        [SerializeField]
        internal Mesh ArrowLongF;
        [SerializeField]
        internal Mesh ArrowMidT;
        [SerializeField]
        internal Mesh ArrowMidF;
        [SerializeField]
        internal Mesh ArrowShortT;
        [SerializeField]
        internal Mesh ArrowShortF;

        [SerializeField]
        internal Transform[] Buttons;


        public GameObject Generate(int from, int to, bool isInverse, Transform parent)
        {

            GameObject clone = Instantiate(gameObject);

            Transform transform = clone.GetComponent<Transform>();
            MeshFilter mesh = clone.GetComponent<MeshFilter>();
            transform.parent = parent;

            if(isInverse)
            {
                mesh.mesh = (Math.Abs(from - to) % 6) switch
                {
                    1 => ArrowShortF,
                    2 => ArrowMidF,
                    4 => ArrowMidF,
                    5 => ArrowShortF,
                    _ => ArrowLongF,
                };
            } else
            {
                mesh.mesh = (Math.Abs(from - to) % 6) switch
                {
                    1 => ArrowShortT,
                    2 => ArrowMidT,
                    4 => ArrowMidT,
                    5 => ArrowShortT,
                    _ => ArrowLongF,
                };
            }

            transform.localPosition = new Vector3(
                (Buttons[from].localPosition.x + Buttons[to].localPosition.x) / 2,
                0.0153f,
                (Buttons[from].localPosition.z + Buttons[to].localPosition.z) / 2
            );
            transform.rotation = Quaternion.Euler(-90, 0, (from + to + (from - to > 0 ? 3 : -3)) % 12 * 30);
            transform.localScale = new Vector3(1, 1, 1);

            return clone;
        }





    }
}
