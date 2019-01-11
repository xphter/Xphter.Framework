using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Collections.Tests {
    [TestClass()]
    public class DirectedGraphTests {
        /// <summary>
        /// Graph: DirectedGraphTest.jpg.
        /// </summary>
        [TestMethod()]
        public void CreateGraphTest() {
            DirectedGraph<int, object> graph = new DirectedGraph<int, object>();
            IEnumerable<IEnumerable<DirectedGraphEdge<int, object>>> paths = null;

            // add edges
            graph.AddEdge(1, 2, null);
            graph.AddEdge(2, 8, null);
            graph.AddEdge(2, 3, null);
            graph.AddEdge(2, 4, null);
            graph.AddEdge(4, 1, null);
            graph.AddEdge(4, 3, null);
            graph.AddEdge(4, 5, null);
            graph.AddEdge(2, 1, null);
            graph.AddEdge(1, 5, null);
            graph.AddEdge(3, 6, null);
            graph.AddEdge(4, 6, null);
            graph.AddEdge(5, 6, null);
            graph.AddEdge(3, 7, null);
            graph.AddEdge(7, 6, null);
            graph.AddEdge(1, 7, null);
            graph.AddEdge(1, 9, null);

            // find out paths
            paths = graph.FindOutPaths(1, 6);
            Assert.AreEqual(8, paths.Count());

            // in and out edges
            Assert.AreEqual(1, graph.GetInEdges(4).Count());
            Assert.AreEqual(4, graph.GetOutEdges(4).Count());

            // remove edge
            graph.RemoveEdge(4, 3);
            paths = graph.FindOutPaths(1, 6);
            Assert.AreEqual(6, paths.Count());

            // add edge
            graph.AddEdge(4, 3, null);
            paths = graph.FindOutPaths(1, 6);
            Assert.AreEqual(8, paths.Count());

            // remove vertex
            graph.RemoveVertex(4);
            paths = graph.FindOutPaths(1, 6);
            Assert.AreEqual(4, paths.Count());

            // clear
            graph.ClearEdges();
            paths = graph.FindOutPaths(1, 6);
            Assert.AreEqual(0, paths.Count());
        }
    }
}
