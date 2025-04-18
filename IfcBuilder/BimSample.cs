using BimBuilder.Classes;
using Microsoft.Isam.Esent.Interop;
using System.Numerics;

namespace BimBuilder
{
    public static class BimSampleFactory
    {
        public static BimContainer Warehouse()
        {
            BimContainer bimContainer = new BimContainer();

            // From the dwg file
            float warehouseWidth = 10200;
            float warehouseLength = 18000;
            float eaveHeight = 3150;
            float peakHeight = eaveHeight + 973;
            float[] bays_width = [3500, 3000, 3500];
            float[] bays_length = [6000, 6000, 6000];
            float roofOverhang = 1400;
            double footing_size = 1000;
            double footing_depth = 600;
            float purlinSpacing = 1000;

            FrameMaterial columnMaterial = FrameMaterialHelper.I.I250;
            FrameMaterial rafterMaterial = FrameMaterialHelper.I.I250;
            FrameMaterial purlinMaterial = FrameMaterialHelper.C.C100;
            CladdingMaterial claddingMaterial = CladdingMaterialHelper.Corrugated;

            // Slab
            bimContainer.Slab = new BimSlab()
            {
                Depth = 200,
                Outline = new List<Vector2>
                {
                    Vector2.Zero,
                    new Vector2(warehouseLength, 0),
                    new Vector2(warehouseLength, warehouseWidth),
                    new Vector2(0, warehouseWidth),
                    Vector2.Zero, // Make sure we fully close it
                }
            };

            List<BimFooting> footings = new List<BimFooting>();
            List<BimFrameMember> columns = new List<BimFrameMember>();
            List<BimFrameMember> rafters = new List<BimFrameMember>();
            List<BimCladding> claddings = new List<BimCladding>();

            float x = 0;
            float startY = -roofOverhang;
            float endY = warehouseWidth + roofOverhang;
            float z = eaveHeight + (float)(0.5 * rafterMaterial.Web);
            float purlinZ = z + (float)(0.5 * rafterMaterial.Web);
            float claddingZ = purlinZ + (float)(purlinMaterial.Web);

            // Footings and Columns
            void AddNewFootingAndColumn(Vector2 pos)
            {
                footings.Add(new BimFooting()
                {
                    Position = pos,
                    Depth = footing_depth,
                    Size = footing_size,
                });

                columns.Add(new BimFrameMember()
                {
                    StartPt = new Vector3(pos.X, pos.Y, 0),
                    EndPt = new Vector3(pos.X, pos.Y, eaveHeight),
                    Rotation = 0,
                    FrameMaterial =columnMaterial,
                });
            }
            
            void AddRafter(float xPos)
            {
                rafters.Add(new BimFrameMember()
                {
                    StartPt = new Vector3(xPos, startY, z),
                    EndPt = new Vector3(xPos, endY, z),
                    FrameMaterial = rafterMaterial
                });
            }

            for (int j = 0; j < 2; j++)
            {
                // Add the first left ones
                AddNewFootingAndColumn(new Vector2(0, j * warehouseWidth));
                AddRafter(0);

                for (int i = 0; i < bays_length.Length; i++)
                {
                    x += bays_length[i];
                    AddNewFootingAndColumn(new Vector2(x, j * warehouseWidth));
                    AddRafter(x);
                }

                x = 0;
            }

            bimContainer.Footings = footings;
            bimContainer.FrameMembers = columns.Concat(rafters).ToList();

            // purlins
            float totalRoofWidth = 2 * roofOverhang + warehouseWidth;
            int numOfPurlins = (int)(totalRoofWidth / purlinSpacing);
            var purlins = new List<BimFrameMember>();
            void AddPurlin(float yPos)
            {
                
            }

            for (int i = 0; i < numOfPurlins; i++)
            {
                
            }

            // roof claddings
            float coveredRoof = 0;

            while (coveredRoof < warehouseLength)
            {
                claddings.Add(new BimCladding()
                {
                    StartPt = new Vector3(coveredRoof, -roofOverhang, claddingZ),
                    EndPt = new Vector3(coveredRoof, warehouseWidth + roofOverhang, claddingZ),
                    CladdingMaterial = claddingMaterial
                });

                coveredRoof += (float)claddingMaterial.CoverWidth;
            }

            bimContainer.Roofs = claddings;

            return bimContainer;
        }
    }
}
