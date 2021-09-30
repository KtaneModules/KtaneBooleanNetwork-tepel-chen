#nullable enable

using KeepCoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BooleanNetwork
{
    internal class ReverseBooleanNetwork
    {
        internal Network network;
        internal List<bool?> endState;
        int step;

        List<int>? _initStateCount;


        internal ReverseBooleanNetwork(Network network, List<bool?> endState, int step)
        {
            this.network = network;
            this.endState = endState;
            this.step = step;
        }

        internal List<int> GetInitStateCount(ILog? logger)
        {
            return _initStateCount ??= GetInitStates(logger).Aggregate(
                new List<int>(new int[network.NumNodes]), 
                (acc, next) => acc.Select((sum, i) => next[i] ? sum + 1 : sum).ToList()
            );
        }

        List<List<bool>> GetInitStates(ILog? logger)
        {
            return Enumerable.Range(0, 1 << network.NumNodes)
                .Select(GenerateInitStateFromInt)
                .Select(state =>
                {
                    var n = new BooleanNetwork(network);
                    n.SetInitState(state);
                    var answer = n.GetState(step);
                    var isValid = Enumerable.Range(0, network.NumNodes).All(i => endState[i] is null || endState[i] == answer[i]);
                    if (logger is ILog log)
                    {
                        log.Log($"Init state {string.Join("", state.Select(s => s ? "T" : "F").ToArray())} had end state {string.Join("", answer.Select(s => s ? "T" : "F").ToArray())}. This is {(isValid ? "valid" : "invalid") }");
                    }
                    return new Tuple<List<bool>, bool>(state, isValid);
                })
                .Where(ends => ends.Item2)
                .Select(ends => ends.Item1)
                .ToList();
        }

        List<bool> GenerateInitStateFromInt(int i)
        {
            return Enumerable.Range(0, network.NumNodes).Select(j => (i & (1 << j)) != 0).ToList();
        }
    }
}
