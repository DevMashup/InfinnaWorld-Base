using System;
using UnityEngine;

namespace DevMashup {
    namespace GamePlay {
        public class Chunk : MonoBehaviour {
            [Tooltip("Active gizmos that represent the area of the chunk")]
            public bool debug = false;
            private byte[] data;
            private int Xpos;
            private int Zpos;
            private Region fatherRegion;
            private bool modified = false;
            private bool changesUnsaved;
            LootTable lootTable;

            /// <summary>
            /// Create a Chunk using a byte[] that contain all the data of the chunk.
            /// </summary>
            /// <param name="b"> data of the chunk</param>
            public Chunk ChunkInit(byte[] b, int x, int z, Region region, bool save) {
                lootTable = new LootTable();
                data = b;
                Xpos = x;
                Zpos = z;
                fatherRegion = region;
                changesUnsaved = save;

                Mesh myMesh = World.Instance.BuildChunk(b);
                GetComponent<MeshFilter>().mesh = myMesh;

                //Assign random color, new material each chunk.
                //mat mymaterial = new mat(Shader.Find("Custom/Geometry/FlatShading"));//Custom/DoubleFaceShader  |   Specular
                //mymat.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                gameObject.AddComponent<MeshCollider>();



                GetComponent<MeshRenderer>().material = World.Instance.scriptableObjects.blocks.Atlas;
                GetComponent<MeshCollider>().sharedMesh = myMesh;
                return this;
            }

            public void Update()
            {
                if(modified)
                {
                    modified = false;
                    changesUnsaved = true;

                    Mesh myMesh = World.Instance.BuildChunk(data);
                    GetComponent<MeshFilter>().mesh = myMesh;
                    GetComponent<MeshCollider>().sharedMesh = myMesh;

                }
            }

            /// <summary>
            /// Call depending of the type of modification to removeTerrain or addTerrain
            /// </summary>
            /// <param name="vertexPoint"></param>
            /// <param name="modification"></param>
            /// <param name="mat"></param>
            public void ModifyTerrain(Vector3 vertexPoint, int modification, int mat = 0)
            {
                if (modification > 0) {
                    AddTerrain(vertexPoint,modification, mat);//A little more costly
                }
                else {
                    RemoveTerrain(vertexPoint,modification);//Less operations
                }
            }

            /// <summary>
            /// Remove terrain in the chunk,
            /// </summary>
            public void RemoveTerrain(Vector3 vertexPoint, int modification) {
                Vector3Int thisVec = new Vector3Int(10,64,10);

                byte matWorks = World.Instance.terrainMap[World.Instance.vectorValues.x, World.Instance.vectorValues.y, World.Instance.vectorValues.x]; //<-This correctly outputs 4, but is only assigned to 1 voxel
                byte mat = World.Instance.terrainMap[Mathf.FloorToInt(vertexPoint.x), Mathf.FloorToInt(vertexPoint.y), Mathf.FloorToInt(vertexPoint.z)]; //<-This doesnt work

                Debug.Log(mat);

                for(int x = 0; x < World.Instance.terrainMap.GetLength(0); x++) {
                    for(int y = 0; y < World.Instance.terrainMap.GetLength(0); y++) {
                        for(int z = 0; z < World.Instance.terrainMap.GetLength(0); z++) {
                            Debug.Log(World.Instance.terrainMap[x, y, z]);
                        }
                    }    
                }
                
                int byteIndex = ((int)vertexPoint.x + (int)vertexPoint.z * Constants.CHUNK_VERTEX_SIZE + (int)vertexPoint.y * Constants.CHUNK_VERTEX_AREA) * Constants.CHUNK_POINT_BYTE;

                int value = data[byteIndex];
                int newValue = Mathf.Clamp(value + modification, 0, 255);

                if (value == newValue) {
                    return;
                }


                data[byteIndex] = (byte)newValue;
                modified = true; //Don't direct change because some vertex are modifier in the same editions, wait to next frame
            }

            public int NormalizeMat(int inversedMat) {
                int mat = 0;

                switch(inversedMat) {
                    case 9:
                        mat = 0;
                        break;
                }

                return mat;
            }

            /// <summary>
            /// Similar to the removeTerrain, but when we add terrain we need indicate a color.
            /// </summary>
            public void AddTerrain(Vector3 vertexPoint,int modification, int mat)
            {
                int isoSurface = World.Instance.debugTest.isoLevel;
                int byteIndex = ((int)vertexPoint.x + (int)vertexPoint.z * Constants.CHUNK_VERTEX_SIZE + (int)vertexPoint.y * Constants.CHUNK_VERTEX_AREA) * Constants.CHUNK_POINT_BYTE ;

                int value = data[byteIndex];
                int newValue = Mathf.Clamp(value + modification, 0, 255);

                if (value == newValue) {
                    return;
                }


                if (value < isoSurface && newValue >= isoSurface) {
                    data[byteIndex + 1] = (byte)mat;
                }

                data[byteIndex] = (byte)newValue;
                modified = true; //Don't direct change because some vertex are modifier in the same editions, wait to next frame
            }

            /// <summary>
            /// Get the material(byte) from a specific point in the chunk
            /// </summary>
            public byte GetMaterial(Vector3 vertexPoint) {
                //int byteIndex = ((int)vertexPoint.x + (int)vertexPoint.z * Constants.CHUNK_VERTEX_SIZE + (int)vertexPoint.y * Constants.CHUNK_VERTEX_AREA) * Constants.CHUNK_POINT_BYTE;
                //return data[byteIndex + 1];
            
                return World.Instance.terrainMap[(int)vertexPoint.x, (int)vertexPoint.y, (int)vertexPoint.z];
            }

            /// <summary>
            /// Save the chunk data in the region if the chunk get some changes.
            /// </summary>
            public void SaveChunkInRegion() {
                if(changesUnsaved) {
                    fatherRegion.saveChunkData(data, Xpos, Zpos);
                }
            }

        #if UNITY_EDITOR
            //Used for visual debug
            void OnDrawGizmos() {
                if (debug) {
                    //Gizmos.color = new Color(1f,0.28f,0f);
                    Gizmos.color = Color.Lerp(Color.red, Color.magenta, ((transform.position.x + transform.position.z) % 100) / 100);


                    Gizmos.DrawWireCube(transform.position,new Vector3(Constants.CHUNK_SIDE, Constants.MAX_HEIGHT * Constants.VOXEL_SIDE, Constants.CHUNK_SIDE));
                }
            }
        #endif
        }
    }
}


