using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Voxel2
{
    struct RayCastHit
    {
        public Vector3 Position;
        public Vector3 Normal;

        public RayCastHit(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }
    }
}
