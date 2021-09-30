using KeepCoding;
using KModkitLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BooleanNetwork
{
    public abstract class BooleanNetworkModuleBase: KtaneModule, ILog
    {
        [SerializeField]
        internal KMSelectable[] Buttons;
        [SerializeField]
        internal MeshRenderer[] ButtonStates;
        [SerializeField]
        internal MeshRenderer[] ButtonColor;
        [SerializeField]
        internal ArrowComponent ArrowComponent;
        [SerializeField]
        internal Material[] ButtonColorMaterial;
        [SerializeField]
        internal Material[] ButtonStateMaterial;
        [SerializeField]
        internal TextMesh[] CBText;

        private Network network;
        internal bool isStrikeAnimation = false;
        protected readonly List<GameObject> arrows = new List<GameObject>();

        protected override void Start()
        {
            base.Start();

            network = GetNetwork();
            foreach (var edge in network.Edges)
            {
                arrows.Add(ArrowComponent.Generate(edge.From, edge.To, edge.IsInv, transform));
            }
            StartCoroutine(FlickerArrows());
            OnColorblindChanged(IsColorblind);


            for (int i = 0; i < 6; i++)
            {
                var j = i;
                ButtonColor[i].material = ButtonColorMaterial[network.AggregatorIdx[i]];
                ButtonStates[i].material = ButtonStateMaterial[GetMaterialIndex(i)];
                Buttons[i].OnInteract += () => { ButtonInteractHandler(j); return false; };
            }
        }

        internal abstract void ResetInput();
        internal abstract Network GetNetwork();
        internal abstract int GetMaterialIndex(int i);
        internal abstract void ButtonInteractHandler(int j);

        private void SetColorblind()
        {
            for (int i = 0; i < 6; i++)
            {
                CBText[i].gameObject.SetActive(true);
                CBText[i].text = (network.AggregatorIdx[i]) switch
                {
                    0 => "R",
                    1 => "G",
                    2 => "B",
                    _ => "?"
                };
            }
        }
        private void RemoveColorblind()
        {
            for (int i = 0; i < 6; i++)
            {
                CBText[i].gameObject.SetActive(false);
            }
        }

        private IEnumerator FlickerArrows()
        {
            while (!IsSolved)
            {
                if (isStrikeAnimation)
                {
                    yield return null;
                    continue;
                }
                var color = new Color32(0, (byte)Random.Range(170, 255), (byte)Random.Range(0, 40), 255);
                foreach (var arrow in arrows)
                {
                    arrow.GetComponent<MeshRenderer>().material.color = color;
                }
                yield return new WaitForSeconds(Random.Range(0.05f, 0.25f));
            }
        }

        public override void OnColorblindChanged(bool isEnabled)
        {
            base.OnColorblindChanged(isEnabled);
            if (isEnabled) SetColorblind();
            else RemoveColorblind();
        } 

        protected IEnumerator SolveAnimation()
        {
            arrows.Shuffle();

            var off = new Color32(12, 12, 12, 255);
            var on = new Color32(0, 255, 40, 255);

            foreach (GameObject arrow in arrows)
            {
                arrow.GetComponent<MeshRenderer>().material.color = off;
            }
            for (int i = 0; i < 3; i++)
            {
                foreach (GameObject arrow in arrows)
                {
                    arrow.GetComponent<MeshRenderer>().material.color = on;
                    yield return new WaitForSeconds(.05f);
                    arrow.GetComponent<MeshRenderer>().material.color = off;
                }
            }

            foreach (GameObject arrow in arrows)
            {
                arrow.GetComponent<MeshRenderer>().material.color = on;
            }
            ResetInput();
        }

        protected IEnumerator StrikeAnimation()
        {
            isStrikeAnimation = true;

            var off = new Color32(12, 12, 12, 255);
            var on = new Color32(224, 0, 0, 255);
            for (int i = 0; i < 5; i++)
            {
                foreach (GameObject arrow in arrows)
                {
                    arrow.GetComponent<MeshRenderer>().material.color = on;
                }
                yield return new WaitForSeconds(.05f);
                foreach (GameObject arrow in arrows)
                {
                    arrow.GetComponent<MeshRenderer>().material.color = off;
                }
                yield return new WaitForSeconds(.05f);
            }
            yield return new WaitForSeconds(.05f);
            ResetInput();

            isStrikeAnimation = false;
            yield return null;
        }
    }
}
