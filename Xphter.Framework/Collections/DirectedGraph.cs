using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a directed-graph.
    /// </summary>
    /// <typeparam name="TVertex">The vertex data type.</typeparam>
    /// <typeparam name="TEdge">The edge dat type.</typeparam>
    public class DirectedGraph<TVertex, TEdge> : IEnumerable<TVertex> {
        /// <summary>
        /// Initialize a new instance of DirectedGraph class.
        /// </summary>
        public DirectedGraph() {
            this.m_vertexes = new List<Vertex>();
            this.m_edges = new List<Edge>();
        }

        private IList<Vertex> m_vertexes;
        private IList<Edge> m_edges;

        /// <summary>
        /// Gets the number of vertexes.
        /// </summary>
        public int VertexesCount {
            get {
                return this.m_vertexes.Count;
            }
        }

        /// <summary>
        /// Gets the number of edges.
        /// </summary>
        public int EdgesCount {
            get {
                return this.m_edges.Count;
            }
        }

        /// <summary>
        /// Perform the action of each in-edges of the specified vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="action"></param>
        private void ForEachInEdges(Vertex vertex, Action<Edge> action) {
            Edge edge = vertex.FirstIn;
            while(edge != null) {
                action(edge);
                edge = edge.NextIn;
            }
        }

        /// <summary>
        /// Perform the action of each out-edges of the specified vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="action"></param>
        private void ForEachOutEdges(Vertex vertex, Action<Edge> action) {
            Edge edge = vertex.FirstOut;
            while(edge != null) {
                action(edge);
                edge = edge.NextOut;
            }
        }

        /// <summary>
        /// Adds a vertex.
        /// </summary>
        /// <param name="data"></param>
        public void AddVertex(TVertex data) {
            if(data == null) {
                throw new ArgumentNullException("data");
            }
            if(this.m_vertexes.Any((item) => object.Equals(item.Data, data))) {
                throw new ArgumentException("The vertex is already existing.", "data");
            }

            this.m_vertexes.Add(new Vertex {
                Data = data,
            });
        }

        /// <summary>
        /// Removes a vertex.
        /// </summary>
        /// <param name="data"></param>
        public void RemoveVertex(TVertex data) {
            if(data == null) {
                throw new ArgumentNullException("data");
            }
            Vertex vertex = this.m_vertexes.FirstOrDefault((item) => object.Equals(item.Data, data));
            if(vertex == null) {
                throw new ArgumentException("The vertex is not existing.", "data");
            }

            this.ForEachInEdges(vertex, (edge) => {
                if(edge.Start.FirstOut == edge) {
                    edge.Start.FirstOut = edge.NextOut;
                } else {
                    Edge outEdge = edge.Start.FirstOut;
                    while(outEdge.NextOut != edge) {
                        outEdge = outEdge.NextOut;
                    }
                    outEdge.NextOut = edge.NextOut;
                }

                this.m_edges.Remove(edge);
            });

            this.ForEachOutEdges(vertex, (edge) => {
                if(edge.End.FirstIn == edge) {
                    edge.End.FirstIn = edge.NextIn;
                } else {
                    Edge inEdge = edge.End.FirstIn;
                    while(inEdge.NextIn != edge) {
                        inEdge = inEdge.NextIn;
                    }
                    inEdge.NextIn = edge.NextIn;
                }

                this.m_edges.Remove(edge);
            });

            this.m_vertexes.Remove(vertex);
        }

        /// <summary>
        /// Removes all vertexes.
        /// </summary>
        public void ClearVertexes() {
            this.m_vertexes.Clear();
            this.m_edges.Clear();
        }

        /// <summary>
        /// Determines whether the graph contains a specific vertex.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ContainsVertex(TVertex data) {
            return this.m_vertexes.Any((item) => object.Equals(item.Data, data));
        }

        /// <summary>
        /// Adds a edge from <paramref name="startData"/> to <paramref name="endData"/>.
        /// </summary>
        /// <param name="startData"></param>
        /// <param name="endData"></param>
        /// <param name="edgeData"></param>
        public void AddEdge(TVertex startData, TVertex endData, TEdge edgeData) {
            Vertex startVertex = this.m_vertexes.FirstOrDefault((item) => object.Equals(item.Data, startData));
            if(startVertex == null) {
                this.m_vertexes.Add(startVertex = new Vertex {
                    Data = startData,
                });
            }

            Vertex endVertex = this.m_vertexes.FirstOrDefault((item) => object.Equals(item.Data, endData));
            if(endVertex == null) {
                this.m_vertexes.Add(endVertex = new Vertex {
                    Data = endData,
                });
            }

            if(startVertex == endVertex) {
                throw new InvalidOperationException("the start vertex and end vertex are the same vertex.");
            }
            if(this.m_edges.Any((item) => item.Start == startVertex && item.End == endVertex)) {
                throw new InvalidOperationException("The specified edge is already existing.");
            }

            Edge edge = new Edge {
                Data = edgeData,
                Start = startVertex,
                End = endVertex,
            };

            if(startVertex.FirstOut == null) {
                startVertex.FirstOut = edge;
            } else {
                Edge lastOutEdge = startVertex.FirstOut;
                while(lastOutEdge.NextOut != null) {
                    lastOutEdge = lastOutEdge.NextOut;
                }
                lastOutEdge.NextOut = edge;
            }

            if(endVertex.FirstIn == null) {
                endVertex.FirstIn = edge;
            } else {
                Edge lastInEdge = endVertex.FirstIn;
                while(lastInEdge.NextIn != null) {
                    lastInEdge = lastInEdge.NextIn;
                }
                lastInEdge.NextIn = edge;
            }

            this.m_edges.Add(edge);
        }

        /// <summary>
        /// Removes the edge from <paramref name="startData"/> to <paramref name="endData"/>.
        /// </summary>
        /// <param name="startData"></param>
        /// <param name="endData"></param>
        public void RemoveEdge(TVertex startData, TVertex endData) {
            Vertex startVertex = this.m_vertexes.FirstOrDefault((item) => object.Equals(item.Data, startData));
            if(startVertex == null) {
                throw new ArgumentException("The specified start vertex is not existing.", "startData");
            }

            Vertex endVertex = this.m_vertexes.FirstOrDefault((item) => object.Equals(item.Data, endData));
            if(endVertex == null) {
                throw new ArgumentException("The specified end vertex is not existing.", "endData");
            }

            Edge edge = this.m_edges.FirstOrDefault((item) => item.Start == startVertex && item.End == endVertex);
            if(edge != null) {
                if(edge.Start.FirstOut == edge) {
                    edge.Start.FirstOut = edge.NextOut;
                } else {
                    Edge outEdge = edge.Start.FirstOut;
                    while(outEdge.NextOut != edge) {
                        outEdge = outEdge.NextOut;
                    }
                    outEdge.NextOut = edge.NextOut;
                }

                if(edge.End.FirstIn == edge) {
                    edge.End.FirstIn = edge.NextIn;
                } else {
                    Edge inEdge = edge.End.FirstIn;
                    while(inEdge.NextIn != edge) {
                        inEdge = inEdge.NextIn;
                    }
                    inEdge.NextIn = edge.NextIn;
                }

                this.m_edges.Remove(edge);
            }
        }

        /// <summary>
        /// Removes all edges.
        /// </summary>
        public void ClearEdges() {
            foreach(Vertex item in this.m_vertexes) {
                item.FirstIn = null;
                item.FirstOut = null;
            }
            this.m_edges.Clear();
        }

        /// <summary>
        /// Determines whether the graph contains a specific edge from <paramref name="startData"/> to <paramref name="endData"/>.
        /// </summary>
        /// <param name="startData"></param>
        /// <param name="endData"></param>
        /// <returns></returns>
        public bool ContainsEdge(TVertex startData, TVertex endData) {
            Vertex startVertex = this.m_vertexes.FirstOrDefault((item) => object.Equals(item.Data, startData));
            if(startVertex == null) {
                throw new ArgumentException("The specified start vertex is not existing.", "startData");
            }

            Vertex endVertex = this.m_vertexes.FirstOrDefault((item) => object.Equals(item.Data, endData));
            if(endVertex == null) {
                throw new ArgumentException("The specified end vertex is not existing.", "endData");
            }

            return this.m_edges.Any((item) => item.Start == startVertex && item.End == endVertex);
        }

        /// <summary>
        /// Determines whether the graph contains a specific edge.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool ContainsEdge(TEdge data) {
            return this.m_edges.Any((item) => object.Equals(item.Data, data));
        }

        /// <summary>
        /// Gets all in-edges of the specified vertex.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IEnumerable<DirectedGraphEdge<TVertex, TEdge>> GetInEdges(TVertex data) {
            if(data == null) {
                throw new ArgumentNullException("data");
            }
            Vertex vertex = this.m_vertexes.FirstOrDefault((item) => object.Equals(item.Data, data));
            if(vertex == null) {
                throw new ArgumentException("The vertex is not existing.", "data");
            }

            Edge edge = vertex.FirstIn;
            ICollection<DirectedGraphEdge<TVertex, TEdge>> edges = new List<DirectedGraphEdge<TVertex, TEdge>>();
            while(edge != null) {
                edges.Add(new DirectedGraphEdge<TVertex, TEdge>(edge.Start.Data, edge.End.Data, edge.Data));
                edge = edge.NextIn;
            }
            return edges;
        }

        /// <summary>
        /// Gets all out-edges of the specified vertex.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public IEnumerable<DirectedGraphEdge<TVertex, TEdge>> GetOutEdges(TVertex data) {
            if(data == null) {
                throw new ArgumentNullException("data");
            }
            Vertex vertex = this.m_vertexes.FirstOrDefault((item) => object.Equals(item.Data, data));
            if(vertex == null) {
                throw new ArgumentException("The vertex is not existing.", "data");
            }

            Edge edge = vertex.FirstOut;
            ICollection<DirectedGraphEdge<TVertex, TEdge>> edges = new List<DirectedGraphEdge<TVertex, TEdge>>();
            while(edge != null) {
                edges.Add(new DirectedGraphEdge<TVertex, TEdge>(edge.Start.Data, edge.End.Data, edge.Data));
                edge = edge.NextOut;
            }
            return edges;
        }

        /// <summary>
        /// Finds all out paths from <paramref name="data"/> to a vertex which mathes <paramref name="predicate"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<IEnumerable<DirectedGraphEdge<TVertex, TEdge>>> FindOutPaths(TVertex data, Func<TVertex, bool> predicate) {
            Vertex vertex = this.m_vertexes.FirstOrDefault((item) => object.Equals(item.Data, data));
            if(vertex == null) {
                throw new ArgumentException("The vertex is not existing.", "data");
            }
            if(predicate == null) {
                throw new ArgumentNullException("predicate");
            }
            if(vertex.FirstOut == null) {
                return Enumerable.Empty<IEnumerable<DirectedGraphEdge<TVertex, TEdge>>>();
            }

            bool isBacking = false;
            Stack<Edge> stack = new Stack<Edge>();
            Edge currentEdge = null, outEdge = null;
            IDictionary<Vertex, bool> values = new Dictionary<Vertex, bool>();
            ICollection<IEnumerable<DirectedGraphEdge<TVertex, TEdge>>> result = new List<IEnumerable<DirectedGraphEdge<TVertex, TEdge>>>();

            stack.Push(vertex.FirstOut);
            while(stack.Count > 0) {
                if(!values.ContainsKey((currentEdge = stack.Peek()).End)) {
                    values[currentEdge.End] = predicate(currentEdge.End.Data);
                }

                if(values[currentEdge.End]) {
                    result.Add(stack.Reverse().Select((item) => new DirectedGraphEdge<TVertex, TEdge>(item.Start.Data, item.End.Data, item.Data)).ToArray());
                    isBacking = true;
                }

                if(isBacking) {
                    stack.Pop();

                    outEdge = currentEdge.NextOut;
                    while(outEdge != null && stack.Any((item) => item.Start == outEdge.End || item.End == outEdge.End)) {
                        outEdge = outEdge.NextOut;
                    }

                    if(outEdge != null) {
                        stack.Push(outEdge);
                        isBacking = false;
                    }
                } else {
                    outEdge = currentEdge.End.FirstOut;
                    while(outEdge != null && stack.Any((item) => item.Start == outEdge.End || item.End == outEdge.End)) {
                        outEdge = outEdge.NextOut;
                    }

                    if(outEdge != null) {
                        stack.Push(outEdge);
                    } else {
                        isBacking = true;
                    }
                }
            }

            return result.OrderBy((item) => item.Count()).ToArray();
        }

        /// <summary>
        /// Finds all out paths from <paramref name="startData"/> to <paramref name="endData"/>.
        /// </summary>
        /// <param name="startData"></param>
        /// <param name="endData"></param>
        /// <returns></returns>
        public IEnumerable<IEnumerable<DirectedGraphEdge<TVertex, TEdge>>> FindOutPaths(TVertex startData, TVertex endData) {
            return this.FindOutPaths(startData, (item) => object.Equals(item, endData));
        }

        #region Vertex and Edge

        private class Vertex {
            public TVertex Data = default(TVertex);
            public Edge FirstIn = null;
            public Edge FirstOut = null;

            public override int GetHashCode() {
                return this.Data != null ? this.Data.GetHashCode() : base.GetHashCode();
            }

            public override string ToString() {
                return this.Data + string.Empty;
            }
        }

        private class Edge {
            public TEdge Data = default(TEdge);
            public Vertex Start = null;
            public Vertex End = null;
            public Edge NextIn = null;
            public Edge NextOut = null;

            public override string ToString() {
                if(this.Data != null) {
                    return string.Format("{0} → {1}: {2}", this.Start.Data, this.End.Data, this.Data);
                } else {
                    return string.Format("{0} → {1}", this.Start.Data, this.End.Data);
                }
            }
        }

        #endregion

        #region IEnumerable<TVertex> Members

        /// <inheritdoc />
        IEnumerator<TVertex> IEnumerable<TVertex>.GetEnumerator() {
            foreach(Vertex item in this.m_vertexes) {
                yield return item.Data;
            }
            yield break;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() {
            foreach(Vertex item in this.m_vertexes) {
                yield return item.Data;
            }
            yield break;
        }

        #endregion
    }

    /// <summary>
    /// Represents the edge in a directed-graph.
    /// </summary>
    /// <typeparam name="TVertex">Vertex data type.</typeparam>
    /// <typeparam name="TEdge">Edge data type.</typeparam>
    public class DirectedGraphEdge<TVertex, TEdge> {
        /// <summary>
        /// Initialize a new instance of DirectedGraphEdge class.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="data"></param>
        internal DirectedGraphEdge(TVertex start, TVertex end, TEdge data) {
            this.Start = start;
            this.End = end;
            this.Data = data;
        }

        /// <summary>
        /// Gets data of the start vertex.
        /// </summary>
        public TVertex Start {
            get;
            private set;
        }

        /// <summary>
        /// Gets data of the end vertex.
        /// </summary>
        public TVertex End {
            get;
            private set;
        }

        /// <summary>
        /// Gets data of this edge.
        /// </summary>
        public TEdge Data {
            get;
            private set;
        }

        /// <inheritdoc />
        public override string ToString() {
            if(this.Data != null) {
                return string.Format("{0} → {1}: {2}", this.Start, this.End, this.Data);
            } else {
                return string.Format("{0} → {1}", this.Start, this.End);
            }
        }
    }
}
