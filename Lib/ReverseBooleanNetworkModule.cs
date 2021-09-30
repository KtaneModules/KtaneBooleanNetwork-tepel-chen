
using KModkitLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BooleanNetwork
{
    internal class ReverseBooleanNetworkModule : BooleanNetworkModuleBase
    {

        private List<int> input = new List<int>();

        internal ReverseBooleanNetwork rbn;


        internal override Network GetNetwork()
        {
            var booleanNetwork = BooleanNetworkGenerator.GenerateNetwork(6);
            booleanNetwork.network.Log(this);
            booleanNetwork.SetInitState(BooleanNetworkGenerator.GenerateState(6));
            var hides = Enumerable.Range(0, 6).ToList().Shuffle().Take(UnityEngine.Random.Range(2,4));
            var endState = booleanNetwork.GetState(3).Select<bool, bool?>((original, i) => hides.Contains(i) ? null : original).ToList();
            Log($"End state: {string.Join(", ", endState.Select(e => e switch {null => "U", true => "T", false => "F" }).ToArray())}");
            rbn = new ReverseBooleanNetwork(booleanNetwork.network, endState, 3);
            Log($"Counts are: {string.Join(", ", rbn.GetInitStateCount(this).Select(count => count.ToString()).ToArray())}");
            return rbn.network;

        }

        internal bool IsCorrect()
        {
            if (input.Count() < 6) return false;
            var count = rbn.GetInitStateCount(this);
            for (int i = 0; i < 5; i++)
            {
                if(count[input[i]] > count[input[i+1]])
                {
                    return false;
                }
            }
            return true;
        }

        internal override void ButtonInteractHandler(int key)
        {
            if (isStrikeAnimation) return;
            if (!input.Contains(key))
            {
                input.Add(key);
                Buttons[key].transform.localPosition += Vector3.down * 0.008f;
                if (input.Count() == 6) Submit();
            }
            ButtonEffect(Buttons[key], 0.2f, KMSoundOverride.SoundEffect.BigButtonPress);
        }
        internal override int GetMaterialIndex(int i)
        {
            return rbn.endState[i] switch
            {
                null => 1,
                true => 0,
                false => 2,
            };
        }

        internal override void ResetInput()
        {
            foreach (int i in input)
            {
                Buttons[i].transform.localPosition += Vector3.up * 0.008f;
            }
            input = new List<int>();
        }

        private void Submit()
        {
            if (IsCorrect()) HandleSolve();
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
            Strike($"Strike! Counts were: {string.Join(", ", rbn.GetInitStateCount(this).Select(count => count.ToString()).ToArray())}, received {string.Join(", ", input.Select(i => (i+1).ToString()).ToArray())}.");

            StartCoroutine(StrikeAnimation());
        }
        public override void OnColorblindChanged(bool b)
        {
            base.OnColorblindChanged(b);
        }
    }
}
