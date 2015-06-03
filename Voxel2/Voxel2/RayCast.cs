using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Voxel2
{
    static class RayCast
    {
        public static Vector3 target;

        public static RayCastHit RayCasting(Camera Camera,float distance)
        {
            Vector3 forward = new Vector3(0, 0, -1);
            Vector3 camPosition = Camera.Position;

            Vector3 transformedForward = Vector3.Transform(new Vector3(0, 0, -1), Camera.cameraRotation);

            for (float i = 0; i < distance*10; i += 1)
            {
                camPosition += 0.1f * transformedForward;
                target = camPosition;

                if (target.X < 0 || target.X > World.Instance.worldX
                    || target.Y < 0 || target.Y > World.Instance.worldY
                    || target.Z < 0 || target.Z > World.Instance.worldZ)
                    return new RayCastHit();

                if (World.Instance.data[(int)Math.Floor(target.X), (int)Math.Floor(target.Y), (int)Math.Floor(target.Z)] != 0)
                    return new RayCastHit(target, -transformedForward);
            }
            return new RayCastHit();
        }
    }
}
