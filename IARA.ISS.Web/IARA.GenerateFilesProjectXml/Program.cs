using System;
using System.IO;
using System.Linq;
using System.Text;

namespace IARA.GenerateFilesProjectXml
{
    internal class Program
    {
        private const string NONE_TEMPLATE = "<None Include=\"{0}\" />";
        private static string root;

        private static string[] EXCLUDE_DIRS = { "wwwroot", "node_modules" };

        private static void Main(string[] args)
        {
            Console.WriteLine("Enter directory to start parsing from: ");
            string directory = Console.ReadLine();

            Console.WriteLine("Enter root directory: ");
            root = Console.ReadLine();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<ItemGroup Label=\"AngularFiles\">");

            DirectoryInfo dir = new DirectoryInfo(directory);

            ParseFilesInDir(dir, ref builder);
            builder.AppendLine("</ItemGroup>");

            string xml = builder.ToString();

            Console.ReadLine();
        }

        private static void ParseFilesInDir(DirectoryInfo dir, ref StringBuilder builder)
        {
            string relativeDirPath = Path.GetRelativePath(root, dir.FullName).TrimStart('/');

            foreach (var fileInfo in dir.GetFiles())
            {
                builder.AppendLine(string.Format(NONE_TEMPLATE, Path.Combine(relativeDirPath, fileInfo.Name)));
            }

            foreach (var childDir in dir.GetDirectories())
            {
                if (!EXCLUDE_DIRS.Contains(childDir.Name))
                {
                    ParseFilesInDir(childDir, ref builder);
                }
            }
        }
    }
}
