using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Voxel2
{
    static class ModifyTerrain
    {
        public static bool BlockInRange(Vector3 position)
        {
            return position.X > 0 && position.X < World.Instance.worldX
                && position.Y > 0 && position.Y< World.Instance.worldY
                && position.Z > 0 && position.Z < World.Instance.worldZ;
        }

        public static void ReplaceBlockCursor(byte block)
        {
            //Replaces the block specified where the mouse cursor is pointing
            RayCastHit hit = RayCast.RayCasting(Camera.Instance, Static.maxRayCastDistance);

            if (hit.Position != Vector3.Zero && BlockInRange(hit.Position)) 
                ReplaceBlockAt(hit, block);
        }

        public static void AddBlockCursor(byte block)
        {
            //Adds the block specified where the mouse cursor is pointing
            RayCastHit hit = RayCast.RayCasting(Camera.Instance,Static.maxRayCastDistance);

            if (hit.Position != Vector3.Zero && BlockInRange(hit.Position))
                AddBlockAt(hit, block);

        }

        public static void ReplaceBlockAt(RayCastHit hit, byte block)
        {
            //removes a block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
            Vector3 position = hit.Position;
            position += hit.Normal * -0.5f;

            SetBlockAt(position, block);
        }

        public static void AddBlockAt(RayCastHit hit, byte block)
        {
            //adds the specified block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
            Vector3 position = hit.Position;
            position += hit.Normal * 0.5f;

            SetBlockAt(position, block);
        }
        
        public static void SetBlockAt(Vector3 position, byte block)
        {
            //sets the specified block at these coordinates
            int x = (int)Math.Floor(position.X);
            int y = (int)Math.Floor(position.Y);
            int z = (int)Math.Floor(position.Z);

            SetBlockAt(x, y, z, block);
        }
        
        public static void SetBlockAt(int x, int y, int z, byte block)
        {
            //adds the specified block at these coordinates
            if (x > 0 && x < World.Instance.worldX && y > 0 && y < World.Instance.worldY && z > 0 && z < World.Instance.worldZ)
            {
                World.Instance.data[x, y, z] = block;
            UpdateChunkAt(x, y, z);
            }
        }

        public static void UpdateChunkAt(int x, int y, int z)
        {
            //Updates the chunk containing this block
            int updateX = (int)Math.Floor((float)x / World.Instance.chunkSize);
            int updateY = (int)Math.Floor((float)y / World.Instance.chunkSize);
            int updateZ = (int)Math.Floor((float)z / World.Instance.chunkSize);

            World.Instance.chunks[updateX, updateY, updateZ].update = true;

            if (x - (World.Instance.chunkSize * updateX) == 0 && updateX != 0)
            {
                World.Instance.chunks[updateX - 1, updateY, updateZ].update = true;
            }

            if (x - (World.Instance.chunkSize * updateX) == 15 && updateX != World.Instance.chunks.GetLength(0) - 1)
            {
                World.Instance.chunks[updateX + 1, updateY, updateZ].update = true;
            }

            if (y - (World.Instance.chunkSize * updateY) == 0 && updateY != 0)
            {
                World.Instance.chunks[updateX, updateY - 1, updateZ].update = true;
            }

            if (y - (World.Instance.chunkSize * updateY) == 15 && updateY != World.Instance.chunks.GetLength(1) - 1)
            {
                World.Instance.chunks[updateX, updateY + 1, updateZ].update = true;
            }

            if (z - (World.Instance.chunkSize * updateZ) == 0 && updateZ != 0)
            {
                World.Instance.chunks[updateX, updateY, updateZ - 1].update = true;
            }

            if (z - (World.Instance.chunkSize * updateZ) == 15 && updateZ != World.Instance.chunks.GetLength(2) - 1)
            {
                World.Instance.chunks[updateX, updateY, updateZ + 1].update = true;
            }

        }


    }
}
