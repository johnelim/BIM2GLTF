using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using Xbim.Common.Geometry;
using Xbim.Ifc;
using Xbim.ModelGeometry.Scene;

namespace Bim2Gltf.Core
{
    using MESH = MeshBuilder<SharpGLTF.Geometry.VertexTypes.VertexPosition>;
    using VERTEX = SharpGLTF.Geometry.VertexTypes.VertexPosition;

    public static class GltfHelper
    {
        public static string Convert(string ifcPath)
        {
            // Open the Ifc Model in memory
            using IfcStore ifcStore = IfcStore.Open(ifcPath);

            // Tessellate the model using Xbim Geometry Engine
            Xbim3DModelContext geometryContext = new Xbim3DModelContext(ifcStore);
            geometryContext.CreateContext();

            // Initialize a Gltf Scene
            SceneBuilder scene = new SceneBuilder(Path.GetFileName(ifcPath));

            // Loop through all the IFC model shape instances
            foreach (XbimShapeInstance? shape in geometryContext.ShapeInstances())
            {
                // Get the 4x4 world transform
                XbimMatrix3D transform = shape.Transformation;

                // Get the underlying geometry definition of the shape
                XbimShapeGeometry geometry = geometryContext.ShapeGeometry(shape);

                // Convert from Xbim geometry to Gltf Mesh
                MESH mesh = CreateMesh(geometry, transform);

                // Define the node holding the transform
                NodeBuilder node = new NodeBuilder("BIM Element");

                scene.AddRigidMesh(mesh, node);
            }

            // Save to GLB / GLTF
            string outputFileName = Path.ChangeExtension(ifcPath, ".glb");
            SharpGLTF.Schema2.ModelRoot model = scene.ToGltf2();
            model.SaveGLB(outputFileName);

            return outputFileName;
        }

        private static MESH CreateMesh(XbimShapeGeometry shape, XbimMatrix3D transform)
        {
            MaterialBuilder material = MaterialBuilder.CreateDefault();
            MESH mesh = new MESH();
            PrimitiveBuilder<MaterialBuilder, VERTEX, SharpGLTF.Geometry.VertexTypes.VertexEmpty, SharpGLTF.Geometry.VertexTypes.VertexEmpty> prim = mesh.UsePrimitive(material);

            List<XbimPoint3D> xbimVertices = shape.Vertices.ToList();

            void AddTriangle(XbimPoint3D v1, XbimPoint3D v2, XbimPoint3D v3)
            {
                v1 = v1 * transform * IfcToGltfWcs;
                v2 = v2 * transform * IfcToGltfWcs;
                v3 = v3 * transform * IfcToGltfWcs;

                prim.AddTriangle(v1.ToMeshVertex(), v2.ToMeshVertex(), v3.ToMeshVertex());
            }

            foreach (WexBimMeshFace? face in shape.Faces)
            {
                int triangles = face.TriangleCount;

                int counter = 0;
                int[] triIndex = new int[3];

                foreach (int index in face.Indices)
                {
                    triIndex[counter] = index;
                    counter++;

                    if (counter == 3)
                    {
                        AddTriangle(xbimVertices[triIndex[0]], xbimVertices[triIndex[1]], xbimVertices[triIndex[2]]);
                        counter = 0;
                    }
                }
            }

            return mesh;
        }

        private static VERTEX ToMeshVertex(this XbimPoint3D p)
        {
            return new VERTEX((float)p.X, (float)p.Y, (float)p.Z);
        }

        /// <summary>
        /// Ifc has out of screen direction as -Y direction
        /// Gltf has out of screen direction as +Y direction
        /// </summary>
        private static XbimMatrix3D IfcToGltfWcs = XbimMatrix3D.CreateRotation(new XbimPoint3D(0, 1, 0), new XbimPoint3D(0, 0, 1));
    }
}
