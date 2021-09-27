using System.Collections.Generic;
using System.Linq;
using KeepCoding;
using KModkitLib;

namespace BooleanNetwork
{
    internal static class BooleanNetworkGenerator
    {
        internal static BooleanNetwork GenerateNetwork(int node_num)
        {

            var loopNodes = Enumerable.Range(0, 6).ToList().Shuffle();
            var edges =
                Enumerable.Range(0, 6)
                .Select(
                    i => new Tuple<int, int>(loopNodes[i], loopNodes[(i + 1) % node_num])
                ).ToList();

            edges.AddRange(Enumerable.Range(0, 5)
                .SelectMany(i => Enumerable.Range(i + 1, 6 - i - 1).Select(j => new Tuple<int, int>(i, j)))
                .Where(uedge => !edges.Any(loopEdge => loopEdge == uedge || new Tuple<int, int>(loopEdge.Item2, loopEdge.Item1) == uedge))
                .Select(uedge => UnityEngine.Random.Range(0, 2) == 0 ? uedge : new Tuple<int, int>(uedge.Item2, uedge.Item1))
                .Take(UnityEngine.Random.Range(3,6)));

            List<BooleanNetwork.NetworkEdge> bnedges = edges.Select(edge => new BooleanNetwork.NetworkEdge(edge.Item1, edge.Item2, UnityEngine.Random.Range(0, 2) == 0)).ToList();
            List<int> aggregators = Enumerable.Range(0, 6).Select(i => UnityEngine.Random.Range(0, BooleanNetwork.AggregatorList.Length)).ToList();

            return new BooleanNetwork(node_num, bnedges, aggregators);
        }

        internal static List<bool> GenerateState(int node_num)
        {
            return Enumerable.Range(0, node_num).Select(i => UnityEngine.Random.Range(0, 2) == 0).ToList();
        }

    }
}
