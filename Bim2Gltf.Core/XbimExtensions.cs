using System.Numerics;
using Xbim.Common.Geometry;

namespace Bim2Gltf.Core
{
    using VERTEX = SharpGLTF.Geometry.VertexTypes.VertexPosition;

    public static class XbimExtensions
    {
        public static VERTEX ToMeshVertex(this XbimPoint3D p)
        {
            return new VERTEX((float)p.X, (float)p.Y, (float)p.Z);
        }

        public static Matrix4x4 ToStandardMatrix(this XbimMatrix3D m)
        {
            XbimVector3D t = m.Translation;

            return new Matrix4x4(
                (float)m.M11, (float)m.M12, (float)m.M13, (float)t.X,
                (float)m.M21, (float)m.M22, (float)m.M23, (float)t.Y,
                (float)m.M31, (float)m.M32, (float)m.M33, (float)t.Z,
                0f, 0f, 0f, 1f
            );
        }
    }
}
