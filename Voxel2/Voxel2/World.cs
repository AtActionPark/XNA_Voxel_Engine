using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Voxel2
{
    //0 : air
    //1 : rock
    //2 : grass
    //3 : dirt
    //4 : water

     public class World
    {
        public byte[,,] data;
        public int worldX = 64;
        public int worldY =32;
        public int worldZ = 64;

        public int chunkSize = 16;
        public Chunk[, ,] chunks;

        private static World instance;
        public static World Instance
        {
            get
            {
                if (instance == null)
                    instance = new World();

                return instance;
            }
        }

        public World()
        {
            data = new byte[worldX, worldY, worldZ];

            SetUpWorld2();

            instance = this;

            chunks = new Chunk[(int)Math.Floor((float)worldX / chunkSize), (int)Math.Floor((float)worldY / chunkSize), (int)Math.Floor((float)worldZ / chunkSize)];

            for (int x = 0; x < chunks.GetLength(0); x++)
                for (int y = 0; y < chunks.GetLength(1); y++)
                    for (int z = 0; z < chunks.GetLength(2); z++)
                    {
                        Chunk chunk = new Chunk(x,y,z);
                        chunks[x, y, z] = chunk;
                    }
        }

        public void Update()
        {
            foreach (Chunk chunk in chunks)
                    chunk.Update();

            //UpdateWater();
        }

        public  void Draw()
        {
            foreach (Chunk chunk in chunks)
                if (chunk.render)
                    chunk.Draw();
        }

        public byte Block(int x, int y, int z)
        {
            if (x >= worldX || x < 0 || y >= worldY || y < 0 || z >= worldZ || z < 0)
                return (byte)0;

            return data[x, y, z];
        }

        int PerlinNoise(int x, int y, int z, float scale, float height, float power)
        {
            float rValue;
            rValue = Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
            rValue *= height;

            if (power != 0)
            {
                rValue =  (float)Math.Pow(rValue, power);
            }

            return (int)rValue;
        }

        float PerlinNoise(float x, float y, float z, float scale, float height, float power, float octave)
        {
            float rValue;
            float finalValue = 0;

            for (int i = 0; i < octave; i++)
            {
                rValue = Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
                rValue *= height;

                if (power != 0)
                    rValue = (float)Math.Pow(rValue, power);
                finalValue+=rValue;
            }
            


            return finalValue;
        }

        float SimplexNoise(float x, float y, float z, int octave)
        {
            float value = 0;
            for (int i = 0; i < octave; i++)
            {
                value += PerlinNoise((int)x * (int)Math.Pow(2, i), (int)y * (int)Math.Pow(2, i), (int)z * (int)Math.Pow(2, i), 10, 10, 1);
            }
            return value;
        }

        public void SetUpWorld()
         {
             for (int x = 0; x < worldX; x++)
                 for (int z = 0; z < worldZ; z++)
                 {
                     int stone = PerlinNoise(x, 0, z, 10, 3, 1.5f)+12;
                     //stone += PerlinNoise(x, 300, z, 20, 4, 0) + 12;
                     int dirt = PerlinNoise(x, 50, z, 50, 10, 1.2f);
                     dirt = PerlinNoise(x, 300, z, 20, 5, 0) + 15;
                     int grass = PerlinNoise(x, 10, z, 50, 3, 1.2f);
                     grass = PerlinNoise(x, 100, z, 20, 5, 0) + 18;

                     for (int y = 0; y < worldY; y++)
                         if (y <= stone)
                             data[x, y, z] = 1;
                         else if (y <= dirt)
                             data[x, y, z] = 3;
                         else if (y <= grass)
                             data[x, y, z] = 2;
                 }
            for (int x = 0; x < worldX; x++)
                 for (int z = 0; z < worldZ; z++)
                     for (int y = 0; y < worldY-1; y++)
                     {
                         if (data[x, y, z] == 3 && data[x, y + 1, z] == 0)
                             data[x, y + 1, z] = 4;
                     }
         }

        public void SetUpWorld2()
        {
            worldX = 128;
            worldY = 128;
            worldZ = 128;
            data = new byte[worldX, worldY, worldZ];

            Camera.Instance.Position = new Vector3(20, worldY-20, 20);
            Camera.Instance.target = new Vector3(worldX/2,worldY/2,worldZ/2);
            Camera.Instance.UpdateViewMatrix();

            GamePlayState.lightPos = new Vector3(worldX/2-100, worldY+10, worldZ/2);
            
            
            for (int x = 0; x < worldX; x++)
                for (int z = 0; z < worldZ; z++)
                    for (int y = 0; y < worldY; y++)
                    {
                        float centerFalloff = 1 - ((x - worldX / 2) * (x - worldX / 2)
                                                + (y - worldX / 2) * (y - worldX / 2)
                                                + (z - worldX / 2) * (z - worldX / 2))
                                                / (float)((worldX / 2) * (worldX / 2) * 3);

                        float plateauFalloff = y > worldY - worldY / 3 ? PerlinNoise(x, 10, z, 10, 10, 1) / (float)10 : 1;

                        float density = PerlinNoise(x, y, z, 10, 20, 1, 2) * (float)Math.Pow(centerFalloff, 10) * plateauFalloff;
                        if (density > 2f)
                        {
                            data[x, y, z] = 1;
                        }
   
                    }
            for (int x = 0; x < worldX; x++)
                for (int z = 0; z < worldZ; z++)
                {
                    int water = PerlinNoise(x, 10, z, 10, 10, 1);
                    int dirt = PerlinNoise(x, 10, z, 20, 20, 1) + 10 + worldY / 2;

                    for (int y = 0; y < worldY - 1; y++)
                    {
                        if (y < water)
                            data[x, y, z] = 4;
                        if (data[x, y, z] != 0 && data[x, y, z] != 4 && y > dirt)
                            data[x, y, z] = 3;
                        if (data[x, y, z] != 0 && data[x, y, z] != 4 && data[x, y + 1, z] == 0)
                            data[x, y, z] = 2;
                    }
                }            
        }


        public void UpdateChunkAt(int x, int y, int z)
        {
            //Updates the chunk containing this block
            int updateX = (int)Math.Floor((float)x / World.Instance.chunkSize);
            int updateY = (int)Math.Floor((float)y / World.Instance.chunkSize);
            int updateZ = (int)Math.Floor((float)z / World.Instance.chunkSize);

            World.Instance.chunks[updateX, updateY, updateZ].update = true;

            if (x - (chunkSize * updateX) == 0 && updateX != 0)
            {
                chunks[updateX - 1, updateY, updateZ].update = true;
            }

            if (x - (chunkSize * updateX) == 15 && updateX != chunks.GetLength(0) - 1)
            {
                chunks[updateX + 1, updateY, updateZ].update = true;
            }

            if (y - (chunkSize * updateY) == 0 && updateY != 0)
            {
                chunks[updateX, updateY - 1, updateZ].update = true;
            }

            if (y - (chunkSize * updateY) == 15 && updateY != chunks.GetLength(1) - 1)
            {
                chunks[updateX, updateY + 1, updateZ].update = true;
            }

            if (z - (chunkSize * updateZ) == 0 && updateZ != 0)
            {
               chunks[updateX, updateY, updateZ - 1].update = true;
            }

            if (z - (chunkSize * updateZ) == 15 && updateZ != chunks.GetLength(2) - 1)
            {
                chunks[updateX, updateY, updateZ + 1].update = true;
            }

        }

        public void UpdateWater()
        {
            for (int x = 1; x < worldX-1; x++)
                 for (int z = 1; z < worldZ-1; z++)
                     for (int y = 1; y < worldY-1; y++)
                         if(data[x,y,z] == 4)
                             if (data[x, y - 1, z] == 0)
                             {
                                 data[x, y - 1, z] = 4;
                                 data[x, y , z] = 0;
                                 UpdateChunkAt(x, y, z);
                             }
                             else if (data[x+1, y-1, z] == 0)
                             {
                                 data[x+1, y-1, z] = 4;
                                 data[x, y, z] = 0;
                             }
                             else if (data[x - 1, y-1, z] == 0)
                             {
                                 data[x - 1, y-1, z] = 4;
                                 data[x, y, z] = 0;
                             }
                             else if (data[x, y-1, z+1] == 0)
                             {
                                 data[x, y-1, z+1] = 4;
                                 data[x, y, z] = 0;
                             }
                             else if (data[x, y-1, z-1] == 0)
                             {
                                 data[x, y-1, z-1] = 4;
                                 data[x, y, z] = 0;
                             }
                             else
                                 if(data[x+1,y,z]==0 || data[x-1,y,z]==0 ||  data[x,y,z+1]==0 ||  data[x,y,z-1]==0 )
                                 data[x, y, z] = 0;
        }

        public void UpdateLight()
        {
            for (int x = 1; x < worldX-1; x++)
                 for (int z = 1; z < worldZ-1; z++)
                     for (int y = 1; y < worldY - 1; y++)
                     {
                         //if(data[x,y,z] == 5)

                     }
        }

        
    }
}
