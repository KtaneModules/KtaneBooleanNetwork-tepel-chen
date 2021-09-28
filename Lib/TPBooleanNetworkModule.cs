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
    public class TPBooleanNetworkModule : TPScript<BooleanNetworkModule>
    {

        public override IEnumerator ForceSolve()
        {
            yield return null;
            Module.ResetInput();
            while (Module.isStrikeAnimation) yield return true;
            var answers = Module.network.GetState(3);
            if (answers.All(b => !b))
            {
                Module.Buttons[0].OnInteract();
                yield return new WaitForSeconds(0.10f);
                Module.Buttons[0].OnInteract();
                yield return UntilSolve();
                yield break;
            }

            for (int i = 0; i < 6; i++)
            {
                if(answers[i])
                {
                    Module.Buttons[i].OnInteract();
                    yield return new WaitForSeconds(0.10f);
                }
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

            if (splitted[0] == "submit" && splitted.Length == 1)
            {
                Module.Buttons[0].OnInteract();
                yield return new WaitForSeconds(0.10f);
                Module.Buttons[0].OnInteract();
                yield break;
            }

            if ((splitted[0] == "submit" || splitted[0] == "press") && splitted.Length == 2 && rxDigits.IsMatch(splitted[1]))
            {
                yield return HandleDigits(splitted[1]);
            }

            if (splitted.Length == 1 && rxDigits.IsMatch(splitted[0]))
            {
                yield return HandleDigits(splitted[0]);
            }


            yield return SendToChatError("Invalid command");
            yield break; 


        }

        private IEnumerator HandleDigits(string v)
        {
            foreach (char c in v)
            {
                Debug.Log(c);
                Module.Buttons[c - '1'].OnInteract();
                yield return new WaitForSeconds(0.10f);
            }
        }

        static readonly Regex rxDigits = new Regex(@"^[1-6]{1,6}$");
    }
}
