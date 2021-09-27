
using KeepCoding;
using KModkitLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BooleanNetwork
{

    public class BooleanNetworkModule : KtaneModule
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

        private readonly List<GameObject> arrows = new List<GameObject>();
        private List<int> input = new List<int>();
        private bool isPressed = false;
        private float lastPressed;
        private BooleanNetwork network;
        private bool isStrikeAnimation = false;


        protected override void Start()
        {
            base.Start();
            network = BooleanNetworkGenerator.GenerateNetwork(6);
            network.network.Log(this);
            network.SetInitState(BooleanNetworkGenerator.GenerateState(6));
            network.Log(this, 3);
            
            foreach(var edge in network.network.Edges)
            {
                arrows.Add(ArrowComponent.Generate(edge.From, edge.To, edge.IsInv, transform));
            }
            StartCoroutine(FlickerArrows());
            for(int i = 0; i < 6; i++)
            {
                var j = i;
                ButtonColor[i].material = ButtonColorMaterial[network.network.AggregatorIdx[i]];
                ButtonStates[i].material = ButtonStateMaterial[network.GetState(0)[i] ? 0 : 1];
                Buttons[i].OnInteract += () => { ButtonInteractHandler(j); return false; };
            }

        }

        protected override void Update()
        {
            base.Update();

            if(!IsSolved && !isStrikeAnimation && isPressed && Time.time -lastPressed > 2)
            {
                Submit();
            }
        }

        private IEnumerator FlickerArrows()
        {
            while(!IsSolved)
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
                yield return new WaitForSeconds(Random.Range(0.05f,0.25f));
            }
        }

        private void ButtonInteractHandler(int key)
        {
            if (isStrikeAnimation) return;
            if(input.Contains(key))
            {
                input.Remove(key);
                Buttons[key].transform.localPosition += Vector3.up * 0.008f;
            } else
            {
                input.Add(key);
                Buttons[key].transform.localPosition += Vector3.down * 0.008f;
            }
            ButtonEffect(Buttons[key], 0.2f, KMSoundOverride.SoundEffect.BigButtonPress);
            lastPressed = Time.time;
            isPressed = true;
        }

        private void Submit()
        {
            var answer = network.GetState(3);
            for(int i = 0; i < 6; i++)
            {
                if(answer[i] ^ input.Contains(i))
                {
                    HandleStrike();
                    return;
                }
            }

            HandleSolve();
        }

        private void HandleSolve()
        {
            Solve("Module Solved!");
            PlaySound("SolveSound");
            StartCoroutine(SolveAnimation());
        }

        private void HandleStrike()
        {
            Strike($"Strike! Expected: {string.Join(", ", network.GetState(3).Select(i => i.ToString()).ToArray())}, received {string.Join(", ", Enumerable.Range(0, 6).Select(i => input.Contains(i).ToString()).ToArray())}.");

            StartCoroutine(StrikeAnimation());
        }

        private IEnumerator SolveAnimation()
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
        }

        private IEnumerator StrikeAnimation()
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
            foreach (int i in input)
            {
                Buttons[i].transform.localPosition += Vector3.up * 0.008f;
            }
            input = new List<int>();
            isPressed = false;

            isStrikeAnimation = false;
            yield return null;
        }
    }
}