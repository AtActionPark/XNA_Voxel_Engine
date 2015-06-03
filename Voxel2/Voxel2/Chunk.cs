using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Voxel2
{
    public class Chunk
    {
        public VertexPositionNormalTexture[] Vertices;
        public int[] indices;

        public List<int> newTriangles = new List<int>();
        public List<Vector3> newVertices = new List<Vector3>();
        public List<Vector3> newNormals = new List<Vector3>();
        public List<Vector2> newUV = new List<Vector2>();

        private float tUnit = .25f;
        private Vector2 tStone = new Vector2(3, 1);
        private Vector2 tGrass = new Vector2(0, 1);
        private Vector2 tGrassTop = new Vector2(2, 2);
        private Vector2 tDirt = new Vector2(1, 1);
        private Vector2 tWater = new Vector2(2, 0);
        private Vector2 tLight = new Vector2(2, 1);
        public bool isEmpty = false;
        
        int faceCount;

        int chunkSize = 16;
        const float offset = 0.00f;

        public int chunkX;
        public int chunkY;
        public int chunkZ;

        public bool update;
        public bool render;

        BoundingBox boundingBox;

        public Chunk(int x, int y, int z)
        {
            chunkX = x * chunkSize;
            chunkY = y * chunkSize;
            chunkZ = z * chunkSize;
            Vector3 min  = new Vector3(chunkX - 0.5f, chunkY + 0.5f, chunkZ - 0.5f);
            Vector3 max = new Vector3(chunkX + chunkSize - 0.5f, chunkY + chunkSize + 0.5f, chunkZ + chunkSize - 0.5f);
            boundingBox = new BoundingBox(min, max);
            GenerateMesh();
        }
        
        public void Update()
        {
            if (update)
            {
                GenerateMesh();
                update = false;
            }
            render = Camera.Instance.boundingFrustrum.Intersects(boundingBox);
        }

        public void UpdateMesh()
        {
            newVertices.Clear();
            newUV.Clear();
            newTriangles.Clear();
            newNormals.Clear();
            faceCount = 0;
        }

        public void CubeTop(int x, int y, int z)
        {
            newVertices.Add(new Vector3(x + chunkX - 0.5f, y + chunkY + 0.5f, z + 1 + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + 1 + chunkX - 0.5f, y + chunkY + 0.5f, z + 1 + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + 1 + chunkX - 0.5f, y + chunkY + 0.5f, z + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + chunkX - 0.5f, y + chunkY + 0.5f, z + chunkZ - 0.5f));

            for(int i = 0;i<4;i++)
                newNormals.Add(new Vector3(0,1,0));

            TextureTopBot(x, y, z);
        }
        public void CubeBottom(int x, int y, int z, byte block)
        {
            newVertices.Add(new Vector3(x + chunkX - 0.5f, y - 1 + chunkY + 0.5f, z + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + 1 + chunkX - 0.5f, y - 1 + chunkY + 0.5f, z + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + 1 + chunkX - 0.5f, y - 1 + chunkY + 0.5f, z + 1 + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + chunkX - 0.5f, y - 1 + chunkY + 0.5f, z + 1 + chunkZ - 0.5f));

            for (int i = 0; i < 4; i++)
                newNormals.Add(new Vector3(0, -1, 0));

            TextureTopBot(x, y, z);
        }
        public void CubeWest(int x, int y, int z, byte block)
        {
            newVertices.Add(new Vector3(x + chunkX - 0.5f, y - 1 + chunkY + 0.5f, z + 1 + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + chunkX - 0.5f, y + chunkY + 0.5f, z + 1 + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + chunkX - 0.5f, y + chunkY + 0.5f, z + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + chunkX - 0.5f, y - 1 + chunkY + 0.5f, z + chunkZ - 0.5f));

            for (int i = 0; i < 4; i++)
                newNormals.Add(new Vector3(-1, 0, 0));

            TextureSide(x, y, z);
        }
        public void CubeNorth(int x, int y, int z, byte block)
        {
            newVertices.Add(new Vector3(x + 1 + chunkX - 0.5f, y - 1 + chunkY + 0.5f, z + 1 + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + 1 + chunkX - 0.5f, y + chunkY + 0.5f, z + 1 + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + chunkX - 0.5f, y + chunkY + 0.5f, z + 1 + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + chunkX - 0.5f, y - 1 + chunkY + 0.5f, z + 1 + chunkZ - 0.5f));

            for (int i = 0; i < 4; i++)
                newNormals.Add(new Vector3(0, 0, 1));

            TextureSide(x, y, z);
        }
        public void CubeSouth(int x, int y, int z, byte block)
        {
            newVertices.Add(new Vector3(x + chunkX - 0.5f, y - 1 + chunkY + 0.5f, z + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + chunkX - 0.5f, y + chunkY + 0.5f, z + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + 1 + chunkX - 0.5f, y + chunkY + 0.5f, z + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + 1 + chunkX - 0.5f, y - 1 + chunkY + 0.5f, z + chunkZ - 0.5f));

            for (int i = 0; i < 4; i++)
                newNormals.Add(new Vector3(0, 0, -1));

            TextureSide(x, y, z);
        }
        public void CubeEast(int x, int y, int z, byte block)
        {
            newVertices.Add(new Vector3(x + 1 + chunkX - 0.5f, y - 1 + chunkY + 0.5f, z + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + 1 + chunkX - 0.5f, y + chunkY + 0.5f, z + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + 1 + chunkX - 0.5f, y + chunkY + 0.5f, z + 1 + chunkZ - 0.5f));
            newVertices.Add(new Vector3(x + 1 + chunkX - 0.5f, y + chunkY - 1 + 0.5f, z + 1 + chunkZ - 0.5f));

            for (int i = 0; i < 4; i++)
                newNormals.Add(new Vector3(1, 0, 0));

            TextureSide(x, y, z);
        }

        public void GenerateLists()
        {
            if (newVertices.Count == 0)
            {
                isEmpty = true;
                return;
            }

            indices = newTriangles.ToArray();
            Vertices = new VertexPositionNormalTexture[newVertices.Count];

            for (int i = 0; i < newVertices.Count; i++)
            {
                Vertices[i].Position = newVertices[i];
                Vertices[i].TextureCoordinate = newUV[i];
                Vertices[i].Normal = newNormals[i];
            }
        }

        private void TextureTopBot(int x, int y, int z)
        {
            Vector2 texturePos = Vector2.Zero;
            if (Block(x, y, z) == 1)
                texturePos = tStone;
            else if (Block(x, y, z) == 2)
                texturePos = tGrassTop;
            else if (Block(x, y, z) == 3)
                texturePos = tDirt;
            else if (Block(x, y, z) == 4)
                texturePos = tWater;
            else if (Block(x, y, z) == 5)
                texturePos = tLight;
            MakeCube(texturePos);
        }
        private void TextureSide(int x, int y, int z)
        {
            Vector2 texturePos = Vector2.Zero;
            if (Block(x, y, z) == 1)
                texturePos = tStone;
            else if (Block(x, y, z) == 2)
                texturePos = tGrass;
            else if (Block(x, y, z) == 3)
                texturePos = tDirt;
            else if (Block(x, y, z) == 4)
                texturePos = tWater;
            else if (Block(x, y, z) == 5)
                texturePos = tLight;
            MakeCube(texturePos);
        }

        void MakeCube(Vector2 texturePos)
        {
            newTriangles.Add(faceCount * 4+2);
            newTriangles.Add(faceCount * 4 + 1);
            newTriangles.Add(faceCount * 4 );
            newTriangles.Add(faceCount * 4+3);
            newTriangles.Add(faceCount * 4 + 2);
            newTriangles.Add(faceCount * 4);

            newUV.Add(new Vector2(tUnit * texturePos.X + tUnit - offset, tUnit * texturePos.Y + offset));
            newUV.Add(new Vector2(tUnit * texturePos.X + tUnit - offset, tUnit * texturePos.Y + tUnit - offset));
            newUV.Add(new Vector2(tUnit * texturePos.X + offset, tUnit * texturePos.Y + tUnit - offset));
            newUV.Add(new Vector2(tUnit * texturePos.X + offset, tUnit * texturePos.Y + offset));

            faceCount++;
        }

        public void GenerateMesh()
        {
            for (int x = 0; x < chunkSize; x++)
                for (int y = 0; y < chunkSize; y++)
                    for (int z = 0; z < chunkSize; z++)
                    {
                        //if the block is solid
                        if (Block(x, y, z) != 0)
                        {
                            //block above is air
                            if (Block(x, y + 1, z) == 0)
                                CubeTop(x, y, z);

                            //block below is air
                            if (Block(x, y - 1, z) == 0)
                                CubeBottom(x, y, z, Block(x, y, z));

                            //block east is air
                            if (Block(x + 1, y, z) == 0)
                                CubeEast(x, y, z, Block(x, y, z));

                            //block west is air
                            if (Block(x - 1, y, z) == 0)
                                CubeWest(x, y, z, Block(x, y, z));

                            //block north is air
                            if (Block(x, y, z + 1) == 0)
                                CubeNorth(x, y, z, Block(x, y, z));

                            //block south is air
                            if (Block(x, y, z - 1) == 0)
                                CubeSouth(x, y, z, Block(x, y, z));
                        }
                    }
            GenerateLists();
            UpdateMesh();
        }

        byte Block(int x, int y, int z)
        {
            return World.Instance.Block(x + chunkX, y + chunkY, z + chunkZ);
        }

        public void Draw()
        {
            if(!isEmpty)
                Static.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,Vertices, 0,Vertices.Length, indices, 0, (int)indices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
        }
    }
}
