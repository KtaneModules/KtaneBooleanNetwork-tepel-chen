
using KeepCoding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BooleanNetwork
{
    internal class TPReverseBooleanNetworkModule : TPScript<ReverseBooleanNetworkModule>
    {

        public override IEnumerator ForceSolve()
        {
            yield return null;
            Module.ResetInput();
            while (Module.isStrikeAnimation) yield return true;
            var counts = Module.rbn.GetInitStateCount(null);
            var order = Enumerable.Range(0, 6).ToList();
            order.Sort((i, j) => counts[i] - counts[j]);

            foreach (int i in order)
            {
                Module.Buttons[i].OnInteract();
            }
            yield return UntilSolve();
        }

        public override IEnumerator Process(string command)
        {
            yield return null;

            Module.ResetInput();
            while (Module.isStrikeAnimation) yield return true;

            string[] splitted = command
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(str => str.Trim().ToLower())
                .ToArray();

            if (splitted.Length == 0)
            {
                yield return SendToChatError("You must specify an argument.");
                yield break;
            }
            if (splitted.Length > 2)
            {
                yield return SendToChatError("Too many arguments.");
                yield break;
            }

            if ((splitted[0] == "submit" || splitted[0] == "press") && splitted.Length == 2 && rxDigits.IsMatch(splitted[1]))
            {
                yield return HandleDigits(splitted[1]);
                yield break;
            }

            if (splitted.Length == 1 && rxDigits.IsMatch(splitted[0]))
            {
                yield return HandleDigits(splitted[0]);
                yield break;
            }


            yield return SendToChatError("Invalid command");
            yield break; 


        }

        private IEnumerator HandleDigits(string v)
        {
            if(v.Distinct().Count() != v.Count()) yield return SendToChatError("Digit must be all unique");
            yield return v.Select(c => Module.Buttons[c - '1']).ToArray();

            if (Module.IsCorrect()) yield return Solve;
            else yield return Strike;
            
        }

        static readonly Regex rxDigits = new Regex(@"^[1-6]{6}$");
    }
}
