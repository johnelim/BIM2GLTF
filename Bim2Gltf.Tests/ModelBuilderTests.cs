using Bim2Gltf.Core;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;

namespace Bim2Gltf.Tests
{
    [TestClass]
    public sealed class ModelBuilderTests
    {
        private static ModelBuilder modelBuilder;

        private int elementCount = 0;
        private int ElementCount => elementCount++;

        [TestMethod]
        public void CreateAllFrameMaterials()
        {
            // Arrange
            modelBuilder = new ModelBuilder("Sample Ifc Elements");

            Vector2 position = new Vector2(0, 0);
            Vector2 offset = new Vector2(3000, 0);

            // Act
            IEnumerable<FrameMaterial> frameMaterials = GetAllFrameMaterialSizes();
            foreach (var frameMaterial in frameMaterials)
            {
                modelBuilder.CreateMember(frameMaterial, position + offset * ElementCount);
            }

            // Assert
            var outputPath = modelBuilder.Save();
            OpenModel(outputPath);
        }

        private static void OpenModel(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            };

            Process.Start(startInfo);
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
