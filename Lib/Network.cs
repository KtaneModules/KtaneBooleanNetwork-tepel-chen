using KeepCoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BooleanNetwork
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
            log.Log($"Aggregators: {string.Join(", ", AggregatorIdx.Select(agg => agg switch { 0 => "AND", 1 => "OR", 2 => "MAJ", _ => "???" }).ToArray())}");
        }


        internal static Func<List<bool>, bool>[] AggregatorList = new Func<List<bool>, bool>[] {
            (List<bool> list) => list.All(i => i),
            (List<bool> list) => list.Any(i => i),
            (List<bool> list) => list.Where(i => i).Count() > list.Count() / 2,
        };
    }
}
