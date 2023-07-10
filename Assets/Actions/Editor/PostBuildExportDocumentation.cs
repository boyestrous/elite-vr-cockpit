﻿using System.IO;
using System.Text.RegularExpressions;
using Markdig;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace EVRC.Core.Actions.Editor
{
    public class PostBuildExportDocumentation : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            var buildDir = Path.GetDirectoryName(report.summary.outputPath);
            var LicenseMPL = Path.Combine(Application.dataPath, "..", "LICENSE-MPL-2.0.md");

            var RootFiles = new string[]
            {
                "README.md",
                "CHANGELOG.md",
                "LICENSE-MPL-2.0.md",
            };

            var DocFiles = new string[]
            {
                "LICENSE.md",
                "Custom.4.0.binds",
                "SOURCE.md",
                "GETTING-STARTED.md",
                "OCULUS-WORKAROUND.md",
            };

            var SpecialFiles = new (string, string)[]
            {
                ( "Textures/Icons/README.md", "Icons.md" ),
                ( "Fonts/README.md", "Fonts.md" ),
            };

            void CopyAndBuildMarkdown(string fromPath, string toName)
            {
                var mdText = File.ReadAllText(fromPath);
                var htmlText = Markdown.ToHtml(Regex.Replace(mdText, "\\(([^)]+).md\\)", "($1.html)"));
                File.WriteAllText(Path.Combine(buildDir, Path.ChangeExtension(toName, "html")), htmlText);
                File.Copy(fromPath, Path.Combine(buildDir, toName), true);
            }

            foreach (var filename in RootFiles)
            {
                CopyAndBuildMarkdown(Path.Combine(Application.dataPath, "..", filename), filename);
                Debug.LogFormat("Copied {0} to root as html/md", filename);
            }

            foreach (var filename in DocFiles)
            {
                CopyAndBuildMarkdown(Path.Combine(Application.dataPath, "Documentation", filename), filename);
                Debug.LogFormat("Copied {0} to root as html/md", filename);
            }

            foreach ((string fromPath, string toName) in SpecialFiles)
            {
                CopyAndBuildMarkdown(Path.Combine(Application.dataPath, fromPath), toName);
                Debug.LogFormat("Copied {0} as {1}/.html to root", fromPath, toName);
            }

            if (!Directory.Exists(Path.Combine(buildDir, "Images")))
            {
                Directory.CreateDirectory(Path.Combine(buildDir, "Images"));
            }

            var imagesPath = Path.Combine(Application.dataPath, "..", "Images");
            string[] imagePaths = Directory.GetFiles(imagesPath, "*.png");
            foreach (var imagePath in imagePaths)
            {
                File.Copy(imagePath, Path.Combine(buildDir, "Images", Path.GetFileName(imagePath)), true);
                Debug.LogFormat("Copied {0} to Images", Path.GetFileName(imagePath));
            }
        }
    }
}
