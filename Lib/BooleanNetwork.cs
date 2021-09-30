using KeepCoding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BooleanNetwork
{
    internal class BooleanNetwork
    {

        private Dictionary<int, List<bool>> _state;
        internal Network network;

        internal BooleanNetwork(int num_nodes, List<NetworkEdge> edges, List<int> aggregatorIdx)
        {
            network = new Network(num_nodes, edges, aggregatorIdx);
        }

        internal BooleanNetwork(Network network)
        {
            this.network = network;
        }

        internal void SetInitState(List<bool> init_state)
        {
            _state = new Dictionary<int, List<bool>>() { { 0, init_state } };
        }
        internal List<bool> GetState(int step)
        {
            if(_state.TryGetValue(step, out var state)){
                return state;
            }
            _state.Add(step, Enumerable.Range(0, network.NumNodes).Select(i => GetInputForNode(step, i)).ToList());
            return _state[step];
        }

        private bool GetInputForNode(int step, int node)
        {
            List<bool> prev = GetState(step - 1);
            return network.Aggregators[node](network.Edges
                .Where(edge => edge.To == node)
                .Select(edge => prev[edge.From] ^ edge.IsInv).ToList());
        }

        internal void Log(ILog logger, int step)
        {
            for(int i = 0; i <= step; i++)
            {
                logger.Log($"Step {i}: {string.Join(", ", GetState(i).Select(b => b.ToString()).ToArray())}");
            }
        }
    }
}
