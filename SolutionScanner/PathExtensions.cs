using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDependencyScanner
{
    public static class PathExtensions
    {
        public static string Normalize(string path)
        {
            string[] parts = path.Split(Path.DirectorySeparatorChar,Path.AltDirectorySeparatorChar);
            List<string> remains = new List<string>();
            int skips = 0;
            for (int i = parts.Length - 1; i > 0; i--)
            {
                if (parts[i] == ".")
                {
                    continue;
                }
                else if (parts[i] == "..")
                {
                    skips++;
                }
                else
                {

                    if (skips > 0)
                    {
                        skips--;
                    }
                    else
                    {
                        remains.Add(parts[i]);
                    }
                }
            }
            remains.Add(parts[0]);
            remains.Reverse();
            return string.Join(Path.DirectorySeparatorChar + "", remains);
        }
    }
}
