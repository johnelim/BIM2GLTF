using Bim2Gltf.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;

namespace Bim2Gltf.Tests
{
    [TestClass]
    public sealed class ModelBuilderTests
    {
        [TestMethod]
        public void SampleIfcToGlb_StructuralColumns()
        {
            // Arrange
            string outputIfc = IfcBuilder.CreateSample_Columns();
            string outputGlb = GltfHelper.ConvertIfc(outputIfc);

            // Act
            TestHelper.OpenModel(outputIfc);
            TestHelper.OpenModel(outputGlb);

            // Assert
            // TODO: Implement reading glb making sure each mesh reflects to each Ifc geometry.
        }

        [TestMethod]
        public void SampleIfcToGlb_Cladding()
        {
            // Arrange
            string outputIfc = IfcBuilder.CreateSample_Cladding();
            string outputGlb = GltfHelper.ConvertIfc(outputIfc);

            // Act
            TestHelper.OpenModel(outputIfc);
            TestHelper.OpenModel(outputGlb);

            // Assert
        }
    }
}
