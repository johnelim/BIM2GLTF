using BimBuilder.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BimBuilder
{
    public static class BimSampleFactory
    {
        public static BimContainer Warehouse()
        {
            BimContainer bimContainer = new BimContainer();

            bimContainer.Slab = new BimSlab()
            {
                Depth = 200,
                Outline = new List<Vector2>
                {
                    Vector2.Zero,
                    new Vector2(18000, 0),
                    new Vector2(18000, 12000),
                    new Vector2(0, 12000),
                    Vector2.Zero, // Make sure we fully close it
                }
            };

            return bimContainer;
        }
    }
}
