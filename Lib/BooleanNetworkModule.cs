
using KModkitLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BooleanNetwork
{

    public class BooleanNetworkModule : BooleanNetworkModuleBase
    {

        private List<int> input = new List<int>();
        private bool isPressed = false;
        private float lastPressed;

        internal BooleanNetwork booleanNetwork;


        internal override Network GetNetwork()
        {
            booleanNetwork = BooleanNetworkGenerator.GenerateNetwork(6);
            booleanNetwork.network.Log(this);
            booleanNetwork.SetInitState(BooleanNetworkGenerator.GenerateState(6));
            booleanNetwork.Log(this, 3);

            return booleanNetwork.network;

        }

        internal bool IsCorrect => !Enumerable.Range(0, 6).Any(i => booleanNetwork.GetState(3)[i] ^ input.Contains(i));

        protected override void Update()
        {
            base.Update();

            if(!IsSolved && !isStrikeAnimation && isPressed && Time.time -lastPressed > 2)
            {
                Submit();
            }
        }

        internal override void ButtonInteractHandler(int key)
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
        internal override int GetMaterialIndex(int i)
        {
            return booleanNetwork.GetState(0)[i] ? 0 : 1;
        }

        internal override void ResetInput()
        {
            foreach (int i in input)
            {
                Buttons[i].transform.localPosition += Vector3.up * 0.008f;
            }
            input = new List<int>();
            isPressed = false;
        }

        private void Submit()
        {
            if (IsCorrect) HandleSolve();
            else HandleStrike();
        }

        private void HandleSolve()
        {
            Solve("Module Solved!");
            PlaySound("SolveSound");
            StartCoroutine(SolveAnimation());
        }

        private void HandleStrike()
        {
            Strike($"Strike! Expected: {string.Join(", ", booleanNetwork.GetState(3).Select(i => i.ToString()).ToArray())}, received {string.Join(", ", Enumerable.Range(0, 6).Select(i => input.Contains(i).ToString()).ToArray())}.");

            StartCoroutine(StrikeAnimation());
        }

        public override void OnColorblindChanged(bool b)
        {
            base.OnColorblindChanged(b);
        }

    }
}