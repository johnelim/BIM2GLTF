using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bim2Gltf.Tests
{
    public static class TestHelper
    {
        public static void OpenModel(string modelPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = modelPath,
                UseShellExecute = true,
                Verb = "open"
            };

            Process.Start(startInfo);
        }

        public static void OpenFolder(string folderPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = folderPath,
                UseShellExecute = true,
                Verb = "open"
            };

            Process.Start(startInfo);
        }
    }
}
