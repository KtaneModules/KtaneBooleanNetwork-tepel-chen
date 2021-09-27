using KeepCoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BooleanNetwork
{
    internal class BooleanNetwork
    {
        internal class NetworkEdge
        {
            internal int From;
            internal int To;
            internal bool IsInv;

            internal NetworkEdge(int from, int to, bool isInv)
            {
                From = from;
                To = to;
                IsInv = isInv;
            }

            public override string ToString()
            {
                return $"{From} -{(IsInv ? "|" : ">")} {To}";
            }
        }

        internal class Network
        {
            internal int NumNodes;
            internal List<NetworkEdge> Edges;
            internal List<int> AggregatorIdx;
            internal List<Func<List<bool>, bool>> Aggregators;

            internal Network(int num_nodes, List<NetworkEdge> edges, List<int> aggregatorIdx)
            {
                NumNodes = num_nodes;
                Edges = edges;
                AggregatorIdx = aggregatorIdx;
                Aggregators = aggregatorIdx.Select(i => AggregatorList[i]).ToList();
            }

            internal void Log(ILog log)
            {
                log.Log("Network info");
                log.Log($"Number of nodes: {NumNodes}");
                log.Log($"Edges: {string.Join(", ", Edges.Select(edge => edge.ToString()).ToArray())}");
                log.Log($"Aggregators: {string.Join(", ", AggregatorIdx.Select(agg => agg switch { 0=>"AND", 1=>"OR", 2=>"MAJ", _ => "???" }).ToArray())}");
            }

        }
        

        private List<List<bool>> _state;
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
            _state = new List<List<bool>>() { init_state };
        }
        internal List<bool> GetState(int step)
        {
            if (_state.Count() > step) return _state[step];
            return Enumerable.Range(0, network.NumNodes).Select(i => GetInputForNode(step, i)).ToList();
        }

        private bool GetInputForNode(int step, int node)
        {
            List<bool> prev = GetState(step - 1);
            return network.Aggregators[node](network.Edges
                .Where(edge => edge.To == node)
                .Select(edge => prev[edge.From] ^ edge.IsInv).ToList());
        }


        internal static Func<List<bool>, bool>[] AggregatorList = new Func<List<bool>, bool>[] {
            (List<bool> list) => list.All(i => i),
            (List<bool> list) => list.Any(i => i),
            (List<bool> list) => list.Where(i => i).Count() > list.Count() / 2,
        };

        internal void Log(ILog logger, int step)
        {
            for(int i = 0; i <= step; i++)
            {
                logger.Log($"Step {i}: {string.Join(", ", GetState(i).Select(b => b.ToString()).ToArray())}");
            }
        }
    }
}
