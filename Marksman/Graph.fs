module Marksman.Graph

open Marksman.MMap

/// Undirected graph
type Graph<'V> when 'V: comparison = { vertices: Set<'V>; edges: MMap<'V, 'V> }

module Graph =
    let empty = { vertices = Set.empty; edges = MMap.empty }

    let hasVertex v g = Set.contains v g.vertices

    let addVertex v g =
        if hasVertex v g then
            g
        else
            { g with vertices = Set.add v g.vertices }

    let addEdge src dest g =
        let g = g |> addVertex src |> addVertex dest
        let edges = g.edges |> MMap.add src dest |> MMap.add dest src
        { g with edges = edges }

    let removeEdge src dest g =
        let edges =
            g.edges |> MMap.removeValue src dest |> MMap.removeValue dest src

        { g with edges = edges }

    let removeVertexWithCallback cb v g =
        let edgesVs = MMap.tryFind v g.edges |> Option.defaultValue Set.empty

        let g =
            Set.fold
                (fun g dest ->
                    cb dest
                    g |> removeEdge v dest |> removeEdge dest v)
                g
                edgesVs

        { g with vertices = Set.remove v g.vertices }

    let removeVertex v g = removeVertexWithCallback ignore v g

    let degree v g = MMap.tryFind v g.edges |> Option.map Set.count
    let isDegree0OrAbsent v g = (degree v g |> Option.defaultValue 0) = 0
