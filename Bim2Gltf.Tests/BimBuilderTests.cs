using Bim2Gltf.Core;
using BimBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bim2Gltf.Tests
{
    [TestClass]
    public sealed class BimBuilderTests
    {
        [TestMethod]
        public void SampleIfcToGlb_StructuralColumns()
        {
            // Arrange
            IfcBuilder ifcBuilder = new IfcBuilder("Sample Ifc - Structural Columns.ifc");
            ifcBuilder.CreateSample_Columns();
            string outputIfc = ifcBuilder.Save();

            // Act
            string outputGlb = GltfHelper.ConvertIfc(outputIfc);
            TestHelper.OpenModel(outputIfc);
            TestHelper.OpenModel(outputGlb);

            // Assert
        }

        [TestMethod]
        public void SampleIfcToGlb_Cladding()
        {
            // Arrange
            IfcBuilder ifcBuilder = new IfcBuilder("Sample Ifc - Corrugated Cladding.ifc");
            ifcBuilder.CreateSample_Cladding();
            string outputIfc = ifcBuilder.Save();

            // Act
            string outputGlb = GltfHelper.ConvertIfc(outputIfc);
            TestHelper.OpenModel(outputIfc);
            TestHelper.OpenModel(outputGlb);

            // Assert
        }

        [TestMethod]
        public void SampleIfcToGlb_Cladding2()
        {
            // Arrange
            IfcBuilder ifcBuilder = new IfcBuilder("Sample Ifc - Corrugated Cladding - Rotated.ifc");
            ifcBuilder.CreateSample_Cladding2();
            string outputIfc = ifcBuilder.Save();

            // Act
            string outputGlb = GltfHelper.ConvertIfc(outputIfc);
            TestHelper.OpenModel(outputIfc);
            TestHelper.OpenModel(outputGlb);

            // Assert
        }

        [TestMethod]
        public void SampleIfcToGlb_Warehouse()
        {
            IfcBuilder ifcBuilder = new IfcBuilder("Sample Ifc - Warehouse.ifc");
            ifcBuilder.CreateSample(BimSampleFactory.Warehouse());
            string outputIfc = ifcBuilder.Save();

            // Act
            string outputGlb = GltfHelper.ConvertIfc(outputIfc);
            TestHelper.OpenModel(outputIfc);
            TestHelper.OpenModel(outputGlb);
        }
    }
}
