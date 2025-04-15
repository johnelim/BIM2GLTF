using Bim2Gltf.Core;

namespace Bim2Gltf.Tests
{
    [TestClass]
    public sealed class BimToGltfTests
    {
        [TestMethod]
        public void CreateSampleIfcAndConvert()
        {
            ModelBuilderTests mbt = new ModelBuilderTests();

            var outputIfc = mbt.CreateAllFrameMaterials();

            string outputGlb = GltfHelper.Convert(outputIfc);

            TestHelper.OpenModel(outputGlb);
        }
    }
}
