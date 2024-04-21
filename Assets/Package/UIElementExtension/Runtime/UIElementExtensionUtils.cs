using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public static class UIElementExtensionUtils
    {

        static readonly Vertex[] k_Vertices = new Vertex[4];
        static readonly ushort[] k_Indices = { 0, 1, 2, 2, 3, 0 };
        public static void FillElementMeshGeneration(MeshGenerationContext mgc, Color topLeft, Color bottomLeft, Color topRight, Color bottomRight)
        {
            Rect r = mgc.visualElement.contentRect;
            if (r.width < 0.01f || r.height < 0.01f)
                return;

            k_Vertices[0].position = new Vector3(0, r.height, Vertex.nearZ);
            k_Vertices[1].position = new Vector3(0, 0, Vertex.nearZ);
            k_Vertices[2].position = new Vector3(r.width, 0, Vertex.nearZ);
            k_Vertices[3].position = new Vector3(r.width, r.height, Vertex.nearZ);

            MeshWriteData mwd = mgc.Allocate(k_Vertices.Length, k_Indices.Length);
            k_Vertices[0].tint = bottomLeft;
            k_Vertices[1].tint = topLeft;
            k_Vertices[2].tint = topRight;
            k_Vertices[3].tint = bottomRight;

            mwd.SetAllVertices(k_Vertices);
            mwd.SetAllIndices(k_Indices);
        }
    }
}
