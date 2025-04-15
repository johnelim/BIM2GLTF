using Bim2Gltf.Core;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;

namespace Bim2Gltf.Tests
{
    [TestClass]
    public sealed class ModelBuilderTests
    {
        [TestMethod]
        public string CreateAllFrameMaterials()
        {
            // Arrange
            IfcBuilder ifcBuilder = new IfcBuilder();

            Vector2 position = new Vector2(0, 0);
            Vector2 offset = new Vector2(3000, 0);

            // Act
            IEnumerable<FrameMaterial> frameMaterials = GetAllFrameMaterialSizes();
            for (int i = 0; i < frameMaterials.Count(); i++)
            {
                ifcBuilder.CreateMember(frameMaterials.ElementAt(i), position + offset * i);
            }

            // Assert
            string outputPath = IfcBuilder.Save();
            TestHelper.OpenModel(outputPath);

            return outputPath;
        }

        private static IEnumerable<FrameMaterial> GetAllFrameMaterialSizes()
        {
            // Using reflection to iterate from all frame material sizes
            Type frameMaterialHelper = typeof(FrameMaterialHelper);

            // Nested classes are the member cross section type (i.e. C or Z channels)
            Type[] crossSectionTypes = frameMaterialHelper.GetNestedTypes();

            foreach (Type crossSectionType in crossSectionTypes)
            {
                string memberType = crossSectionType.Name;

                // Inner loop are the exact sizes (100, 200, 300 etc.)
                foreach (PropertyInfo crossSectionSize in crossSectionType.GetProperties(BindingFlags.Public | BindingFlags.Static))
                {
                    if (crossSectionSize.GetValue(null) is FrameMaterial value)
                    {
                        yield return value;
                    }
                }
            }
        }
    }
}
