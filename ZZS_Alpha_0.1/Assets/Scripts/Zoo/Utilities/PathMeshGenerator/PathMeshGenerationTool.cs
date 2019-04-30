using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zoo.Utilities.PathMeshGenerator
{
    public static class PathMeshGenerationTool
    {
        public static Dictionary<string, Mesh> PathMeshDictionary = new Dictionary<string, Mesh>();

        private static readonly string[] PathCases =
        {
            "single", "full", "straight", "end", "slope", "corner","tjunction", "fourway",
            "fullcorner", "halftjunction", "3quarterfourway","halffourway", "diagfourway",
            "quarterfourway","outsidecorner", "quartertjunctiontop", "quartertjunctionbottom"
        };

        public static void Initialize()
        {
            GeneratePathMeshes();

        }

        private static void GeneratePathMeshes()
        {
            foreach (var key in PathCases)
            {
                GenerateMesh(key);
            }

        }

        private static void GenerateMesh(string key)
        {
            var mesh = new Mesh();
            var verts = new List<Vector3>();
            var tris = new List<int>();
            var uvs = new List<Vector2>();

            switch (key)
            {
                case "single":
                    {
                        verts.Add(new Vector3(0.1f, 0, 0.1f));
                        verts.Add(new Vector3(0.1f, 0, 0.4f));
                        verts.Add(new Vector3(0.4f, 0, 0.4f));
                        verts.Add(new Vector3(0.4f, 0, 0.1f));

                        tris.Add(0);
                        tris.Add(1);
                        tris.Add(2);

                        tris.Add(0);
                        tris.Add(2);
                        tris.Add(3);

                        // UVs
                        //uvs.Add(new Vector2(0.2f, 0.2f));
                        //uvs.Add(new Vector2(0.2f, 0.8f));
                        //uvs.Add(new Vector2(0.8f, 0.8f));
                        //uvs.Add(new Vector2(0.8f, 0.2f));
                        break;
                    }
                case "full":
                    {
                        verts.Add(new Vector3(0f, 0, 0f));
                        verts.Add(new Vector3(0f, 0, 0.5f));
                        verts.Add(new Vector3(0.5f, 0, 0.5f));
                        verts.Add(new Vector3(0.5f, 0, 0f));

                        tris.Add(0);
                        tris.Add(1);
                        tris.Add(2);

                        tris.Add(0);
                        tris.Add(2);
                        tris.Add(3);

                        // UVs
                        //uvs.Add(new Vector2(0, 0));
                        //uvs.Add(new Vector2(0, 1));
                        //uvs.Add(new Vector2(1, 1));
                        //uvs.Add(new Vector2(1, 0));
                        break;
                    }
                case "straight":
                    {
                        verts.Add(new Vector3(0, 0, 0.1f));
                        verts.Add(new Vector3(0, 0, 0.4f));
                        verts.Add(new Vector3(0.5f, 0, 0.4f));
                        verts.Add(new Vector3(0.5f, 0, 0.1f));

                        tris.Add(0);
                        tris.Add(1);
                        tris.Add(2);

                        tris.Add(0);
                        tris.Add(2);
                        tris.Add(3);

                        // UVs
                        //uvs.Add(new Vector2(0, 0.2f));
                        //uvs.Add(new Vector2(0, 0.8f));
                        //uvs.Add(new Vector2(1, 0.8f));
                        //uvs.Add(new Vector2(1, 0.2f));
                        break;
                    }
                case "slope":
                    {
                        verts.Add(new Vector3(0, 0, 0.1f));
                        verts.Add(new Vector3(0, 0, 0.4f));
                        verts.Add(new Vector3(.5f, .25f, 0.4f));
                        verts.Add(new Vector3(.5f, .25f, 0.1f));

                        tris.Add(0);
                        tris.Add(1);
                        tris.Add(2);

                        tris.Add(0);
                        tris.Add(2);
                        tris.Add(3);

                        // UVs
                        //uvs.Add(new Vector2(0, 0));
                        //uvs.Add(new Vector2(0, 1));
                        //uvs.Add(new Vector2(1, 1));
                        //uvs.Add(new Vector2(1, 0));
                        break;
                    }
                case "end":
                    {
                        verts.Add(new Vector3(0, 0, 0.1f));
                        verts.Add(new Vector3(0, 0, 0.4f));
                        verts.Add(new Vector3(0.4f, 0, 0.4f));
                        verts.Add(new Vector3(0.4f, 0, 0.1f));

                        tris.Add(0);
                        tris.Add(1);
                        tris.Add(2);

                        tris.Add(0);
                        tris.Add(2);
                        tris.Add(3);

                        // UVs
                        //uvs.Add(new Vector2(0, 0));
                        //uvs.Add(new Vector2(0, 1));
                        //uvs.Add(new Vector2(1, 1));
                        //uvs.Add(new Vector2(1, 0));
                        break;
                    }
                case "corner":
                    {
                        verts.Add(new Vector3(0f, 0, 0.1f));
                        verts.Add(new Vector3(0f, 0, 0.4f));
                        verts.Add(new Vector3(0.1f, 0, 0.4f));
                        verts.Add(new Vector3(0.1f, 0, 0.5f));
                        verts.Add(new Vector3(0.4f, 0, 0.5f));
                        verts.Add(new Vector3(0.4f, 0, 0.1f));

                        tris.Add(0);
                        tris.Add(1);
                        tris.Add(2);

                        tris.Add(2);
                        tris.Add(3);
                        tris.Add(4);

                        tris.Add(2);
                        tris.Add(4);
                        tris.Add(5);

                        tris.Add(2);
                        tris.Add(5);
                        tris.Add(0);


                        //UVs
                        //uvs.Add(new Vector2(0.2f, 0.2f));
                        //uvs.Add(new Vector2(0.2f, 1));
                        //uvs.Add(new Vector2(0.8f, 1));
                        //uvs.Add(new Vector2(0.8f, 0.8f));
                        //uvs.Add(new Vector2(1, 0.8f));
                        //uvs.Add(new Vector2(1, 0.2f));
                        break;
                    }
                case "tjunction":
                    {
                        verts.Add(new Vector3(0f, 0, 0.1f));
                        verts.Add(new Vector3(0f, 0, 0.4f));
                        verts.Add(new Vector3(0.1f, 0, 0.4f));
                        verts.Add(new Vector3(0.1f, 0, 0.5f));
                        verts.Add(new Vector3(0.4f, 0, 0.5f));
                        verts.Add(new Vector3(0.4f, 0, 0f));
                        verts.Add(new Vector3(0.1f, 0, 0f));
                        verts.Add(new Vector3(0.1f, 0, 0.1f));

                        tris.Add(0);
                        tris.Add(1);
                        tris.Add(2);

                        tris.Add(0);
                        tris.Add(2);
                        tris.Add(7);

                        tris.Add(2);
                        tris.Add(3);
                        tris.Add(4);

                        tris.Add(2);
                        tris.Add(4);
                        tris.Add(5);

                        tris.Add(2);
                        tris.Add(4);
                        tris.Add(7);

                        tris.Add(4);
                        tris.Add(5);
                        tris.Add(7);

                        tris.Add(5);
                        tris.Add(6);
                        tris.Add(7);

                        //UVs
                        //uvs.Add(new Vector2(0, 0.2f));
                        //uvs.Add(new Vector2(0, 0.8f));
                        //uvs.Add(new Vector2(0.2f, 0.8f));
                        //uvs.Add(new Vector2(0.2f, 1f));
                        //uvs.Add(new Vector2(0.8f, 1f));
                        //uvs.Add(new Vector2(0.8f, 0.8f));
                        //uvs.Add(new Vector2(1, 0.8f));
                        //uvs.Add(new Vector2(1, 0.2f));
                        break;
                    }
                case "fourway":
                    {
                        verts.Add(new Vector3(0f, 0, 0.1f));
                        verts.Add(new Vector3(0f, 0, 0.4f));
                        verts.Add(new Vector3(0.1f, 0, 0.4f));
                        verts.Add(new Vector3(0.1f, 0, 0.5f));
                        verts.Add(new Vector3(0.4f, 0, 0.5f));
                        verts.Add(new Vector3(0.4f, 0, 0.4f));
                        verts.Add(new Vector3(0.5f, 0, 0.4f));
                        verts.Add(new Vector3(0.5f, 0, 0.1f));
                        verts.Add(new Vector3(0.4f, 0, 0.1f));
                        verts.Add(new Vector3(0.4f, 0, 0.0f));
                        verts.Add(new Vector3(0.1f, 0, 0.0f));
                        verts.Add(new Vector3(0.1f, 0, 0.1f));

                        tris.Add(0);
                        tris.Add(1);
                        tris.Add(2);

                        tris.Add(0);
                        tris.Add(2);
                        tris.Add(11);

                        tris.Add(2);
                        tris.Add(3);
                        tris.Add(4);

                        tris.Add(2);
                        tris.Add(4);
                        tris.Add(5);

                        tris.Add(5);
                        tris.Add(6);
                        tris.Add(7);

                        tris.Add(5);
                        tris.Add(7);
                        tris.Add(8);

                        tris.Add(8);
                        tris.Add(9);
                        tris.Add(10);

                        tris.Add(8);
                        tris.Add(10);
                        tris.Add(11);

                        tris.Add(11);
                        tris.Add(2);
                        tris.Add(5);

                        tris.Add(11);
                        tris.Add(5);
                        tris.Add(8);

                        //UVs
                        //uvs.Add(new Vector2(0, 0.2f));
                        //uvs.Add(new Vector2(0, 0.8f));
                        //uvs.Add(new Vector2(0.2f, 0.8f));
                        //uvs.Add(new Vector2(0.2f, 1f));
                        //uvs.Add(new Vector2(0.8f, 1f));
                        //uvs.Add(new Vector2(0.8f, 0.8f));
                        //uvs.Add(new Vector2(1, 0.8f));
                        //uvs.Add(new Vector2(1, 0.2f));
                        //uvs.Add(new Vector2(0.8f, 0.2f));
                        //uvs.Add(new Vector2(0.8f, 0));
                        //uvs.Add(new Vector2(0.2f, 0));
                        //uvs.Add(new Vector2(0.2f, 0.2f));
                        break;
                    }
                case "fullcorner":
                    {
                        verts.Add(new Vector3(0f, 0, 0.1f));
                        verts.Add(new Vector3(0f, 0, 0.5f));
                        verts.Add(new Vector3(0.4f, 0, 0.5f));
                        verts.Add(new Vector3(0.4f, 0, 0.1f));


                        tris.Add(0);
                        tris.Add(1);
                        tris.Add(2);

                        tris.Add(0);
                        tris.Add(2);
                        tris.Add(3);

                        //UVs
                        //uvs.Add(new Vector2(0.2f, 0.2f));
                        //uvs.Add(new Vector2(0.2f, 1f));
                        //uvs.Add(new Vector2(1f, 1f));
                        //uvs.Add(new Vector2(1f, 0.2f));

                        break;
                    }
                case "halftjunction":
                    {
                        verts.Add(new Vector3(0f, 0, 0f));
                        verts.Add(new Vector3(0f, 0, 0.5f));
                        verts.Add(new Vector3(0.4f, 0, 0.5f));
                        verts.Add(new Vector3(0.4f, 0, 0f));


                        tris.Add(0);
                        tris.Add(1);
                        tris.Add(2);

                        tris.Add(0);
                        tris.Add(2);
                        tris.Add(3);

                        //UVs
                        //uvs.Add(new Vector2(0.2f, 0f));
                        //uvs.Add(new Vector2(0.2f, 1f));
                        //uvs.Add(new Vector2(1f, 1f));
                        //uvs.Add(new Vector2(1f, 0f));

                        break;
                    }
                case "3quarterfourway":
                {
                    verts.Add(new Vector3(0f, 0, 0f));
                    verts.Add(new Vector3(0f, 0, 0.5f));
                    verts.Add(new Vector3(0.5f, 0, 0.5f));
                    verts.Add(new Vector3(0.5f, 0, 0.1f));
                    verts.Add(new Vector3(0.4f, 0, 0.1f));
                    verts.Add(new Vector3(0.4f, 0, 0f));


                    tris.Add(0);
                    tris.Add(1);
                    tris.Add(2);

                    tris.Add(0);
                    tris.Add(2);
                    tris.Add(4);

                    tris.Add(2);
                    tris.Add(3);
                    tris.Add(4);

                    tris.Add(4);
                    tris.Add(5);
                    tris.Add(0);

                        //UVs
                    //uvs.Add(new Vector2(0f, 0f));
                    //uvs.Add(new Vector2(0f, 0.8f));
                    //uvs.Add(new Vector2(0.2f, 0.8f));
                    //uvs.Add(new Vector2(0.2f, 1f));
                    //uvs.Add(new Vector2(1f, 1f));
                    //uvs.Add(new Vector2(1f, 0f));

                    break;
                }
                case "halffourway": 
                {
                    verts.Add(new Vector3(0f, 0, 0f));
                    verts.Add(new Vector3(0f, 0, 0.5f));
                    verts.Add(new Vector3(0.4f, 0, 0.5f));
                    verts.Add(new Vector3(0.4f, 0, 0.4f));
                    verts.Add(new Vector3(0.5f, 0, 0.4f));
                    verts.Add(new Vector3(0.5f, 0, 0.1f));
                    verts.Add(new Vector3(0.4f, 0, 0.1f));
                    verts.Add(new Vector3(0.4f, 0, 0f));

                    tris.Add(0);
                    tris.Add(1);
                    tris.Add(2);

                    tris.Add(0);
                    tris.Add(2);
                    tris.Add(3);

                    tris.Add(0);
                    tris.Add(3);
                    tris.Add(6);

                    tris.Add(3);
                    tris.Add(4);
                    tris.Add(5);

                    tris.Add(3);
                    tris.Add(5);
                    tris.Add(6);

                    tris.Add(6);
                    tris.Add(7);
                    tris.Add(0);

                    //UVs
                    //uvs.Add(new Vector2(0f, 0f));
                    //uvs.Add(new Vector2(0f, 0.8f));
                    //uvs.Add(new Vector2(0.2f, 0.8f));
                    //uvs.Add(new Vector2(0.2f, 1f));
                    //uvs.Add(new Vector2(0.8f, 1f));
                    //uvs.Add(new Vector2(0.8f, 0.8f));
                    //uvs.Add(new Vector2(1f, 0.8f));
                    //uvs.Add(new Vector2(1f, 0f));

                        break;
                }
                case "diagfourway":
                {
                    verts.Add(new Vector3(0f, 0, 0.1f));
                    verts.Add(new Vector3(0f, 0, 0.5f));
                    verts.Add(new Vector3(0.4f, 0, 0.5f));
                    verts.Add(new Vector3(0.4f, 0, 0.4f));
                    verts.Add(new Vector3(0.5f, 0, 0.4f));
                    verts.Add(new Vector3(0.5f, 0, 0f));
                    verts.Add(new Vector3(0.1f, 0, 0f));
                    verts.Add(new Vector3(0.1f, 0, 0.1f));

                    tris.Add(0);
                    tris.Add(1);
                    tris.Add(2);

                    tris.Add(0);
                    tris.Add(2);
                    tris.Add(7);

                    tris.Add(2);
                    tris.Add(3);
                    tris.Add(7);

                    tris.Add(3);
                    tris.Add(4);
                    tris.Add(5);

                    tris.Add(3);
                    tris.Add(5);
                    tris.Add(6);

                    tris.Add(3);
                    tris.Add(6);
                    tris.Add(7);

                    //UVs
                    //uvs.Add(new Vector2(0f, 0f));
                    //uvs.Add(new Vector2(0f, 0.8f));
                    //uvs.Add(new Vector2(0.2f, 0.8f));
                    //uvs.Add(new Vector2(0.2f, 1f));
                    //uvs.Add(new Vector2(0.8f, 1f));
                    //uvs.Add(new Vector2(0.8f, 0.8f));
                    //uvs.Add(new Vector2(1f, 0.8f));
                    //uvs.Add(new Vector2(1f, 0f));

                    break;
                }
                case "quarterfourway":
                {
                    verts.Add(new Vector3(0f, 0, 0.1f));
                    verts.Add(new Vector3(0f, 0, 0.5f));
                    verts.Add(new Vector3(0.4f, 0, 0.5f));
                    verts.Add(new Vector3(0.4f, 0, 0.4f));
                    verts.Add(new Vector3(0.5f, 0, 0.4f));
                    verts.Add(new Vector3(0.5f, 0, 0.1f));
                    verts.Add(new Vector3(0.4f, 0, 0.1f));
                    verts.Add(new Vector3(0.4f, 0, 0f));
                    verts.Add(new Vector3(0.1f, 0, 0f));
                    verts.Add(new Vector3(0.1f, 0, 0.1f));

                    tris.Add(0);
                    tris.Add(1);
                    tris.Add(2);

                    tris.Add(0);
                    tris.Add(2);
                    tris.Add(9);

                    tris.Add(2);
                    tris.Add(3);
                    tris.Add(9);

                    tris.Add(3);
                    tris.Add(6);
                    tris.Add(9);


                    tris.Add(3);
                    tris.Add(4);
                    tris.Add(5);

                    tris.Add(3);
                    tris.Add(5);
                    tris.Add(6);

                    tris.Add(6);
                    tris.Add(7);
                    tris.Add(8);

                    tris.Add(6);
                    tris.Add(8);
                    tris.Add(9);

                    //UVs
                    //uvs.Add(new Vector2(0f, 0.2f));
                    //uvs.Add(new Vector2(0f, 0.8f));
                    //uvs.Add(new Vector2(0.2f, 0.8f));
                    //uvs.Add(new Vector2(0.2f, 1f));
                    //uvs.Add(new Vector2(0.8f, 1f));
                    //uvs.Add(new Vector2(0.8f, 0.8f));
                    //uvs.Add(new Vector2(1f, 0.8f));
                    //uvs.Add(new Vector2(1f, 0f));
                    //uvs.Add(new Vector2(0.2f, 0f));
                    //uvs.Add(new Vector2(0.2f, 0.2f));

                    break;
                }
                case "outsidecorner":
                    {                   
                        
                        // Do some math and make the curve verts
                        for (var i = 0f; i < (Mathf.PI / 2); i += 0.05f)
                    {
                       verts.Add(new Vector3(Mathf.Cos(i) / 2, 0f, Mathf.Sin(i) / 2));
                    }

                    // Create top left corner
                    verts.Add(new Vector3(0.5f, 0, 0.5f));  // 0

                        // Reverse, so top right is now 0.
                        verts.Reverse();

                        // Create each tri
                        for (var i = 0; i < verts.Count - 1; i++)
                    {
                        tris.Add(0);
                        tris.Add(i + 1);
                        tris.Add(i);
                    }

                    //UVs


                    break;
                }
                case "quartertjunctiontop":
                {
                    verts.Add(new Vector3(0f, 0, 0f));
                    verts.Add(new Vector3(0f, 0, 0.4f));
                    verts.Add(new Vector3(0.1f, 0, 0.4f));
                    verts.Add(new Vector3(0.1f, 0, 0.5f));
                    verts.Add(new Vector3(0.4f, 0, 0.5f));
                    verts.Add(new Vector3(0.4f, 0, 0f));


                    tris.Add(0);
                    tris.Add(1);
                    tris.Add(2);

                    tris.Add(0);
                    tris.Add(2);
                    tris.Add(5);

                    tris.Add(2);
                    tris.Add(3);
                    tris.Add(4);

                    tris.Add(2);
                    tris.Add(4);
                    tris.Add(5);


                        break;
                }
                case "quartertjunctionbottom":
                {
                    verts.Add(new Vector3(0f, 0, 0.1f));
                    verts.Add(new Vector3(0f, 0, 0.5f));
                    verts.Add(new Vector3(0.4f, 0, 0.5f));
                    verts.Add(new Vector3(0.4f, 0, 0f));
                    verts.Add(new Vector3(0.1f, 0, 0f));
                    verts.Add(new Vector3(0.1f, 0, 0.1f));


                    tris.Add(0);
                    tris.Add(1);
                    tris.Add(2);

                    tris.Add(0);
                    tris.Add(2);
                    tris.Add(5);

                    tris.Add(2);
                    tris.Add(3);
                    tris.Add(5);

                    tris.Add(3);
                    tris.Add(4);
                    tris.Add(5);


                    break;
                }
            }


            // Create & add UVs like a competent programmer
            foreach (var vert in verts)
            {
                uvs.Add(new Vector2(vert.x * 2f, vert.y * 2f));
            }

            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            PathMeshDictionary.Add(key, mesh);

        }



    }
}
