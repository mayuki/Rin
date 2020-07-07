using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Rin
{
    public static class Resources
    {
        private static Dictionary<string, Func<Stream>> _resourceNameByPath = new Dictionary<string, Func<Stream>>();
        private static ZipArchive _zipArchive;

        static Resources()
        {
            var asm = typeof(Resources).Assembly;
            var resourceZipStream = asm.GetManifestResourceStream(typeof(Resources).Assembly.GetName().Name + ".Resources.zip");
            _zipArchive = new ZipArchive(resourceZipStream, ZipArchiveMode.Read);
            _resourceNameByPath = _zipArchive.Entries.Where(x => x.Name != "").ToDictionary(k => k.FullName, v => (Func<Stream>)(() => v.Open()));
        }

        public static bool TryOpen(string name, out Stream stream, out string contentType)
        {
            if (_resourceNameByPath.TryGetValue(name, out var func))
            {
                stream = func();
                contentType = GetContentType(name);
                return true;
            }

            stream = null;
            contentType = null;
            return false;
        }

        public static string GetContentType(string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".html": return "text/html; charset=utf-8";
                case ".txt": return "text/plain; charset=utf-8";
                case ".png": return "image/png";
                case ".jpg": return "image/jpeg";
                case ".css": return "text/css";
                case ".js": return "application/javascript";
                case ".json": return "application/json";
                default: return "application/octet-stream";
            }
        }
    }
}
