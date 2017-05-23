using System.Reflection;
using System.IO;
using SkiaSharp;
using System.Xml;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml.Internals;

namespace App_112GW
{
    class ResourceLoader
    {
        public delegate bool ProcessFile(string Filename);

        private     ProcessFile _FileFunction;
        protected   ProcessFile FileFunction
        {
            get
            {
                return _FileFunction;
            }
            set
            {
                _FileFunction = value;
                Execute();
            }
        }

        protected Stream GetStream(string pPath)
        {
            return mAssembly.GetManifestResourceStream(pPath);
        }
        protected string GetFilename(string pPath)
        {
            //Remove extension
            pPath = pPath.Remove(pPath.LastIndexOf('.'));
            pPath = pPath.Substring(pPath.LastIndexOf('.')+1);
            return pPath;
        }

        protected Assembly mAssembly;
        protected void Execute()
        {
            mAssembly = typeof(ResourceLoader).GetTypeInfo().Assembly;
            var files = mAssembly.GetManifestResourceNames();
            foreach (var file in files)
                FileFunction(file);
        }

        protected ResourceLoader()
        {
            FileFunction = (v) => true;
        }
        public ResourceLoader(ProcessFile pFileCallback)
        {
            FileFunction = pFileCallback;
            Execute();
        }
    }
    class ImageLoader: ResourceLoader
    {
        public delegate bool    ProcessImage(string Name, SKImage Image);
        private                 ProcessImage mImageFunction;
        private bool HasImageExtension(string source)
        {
            return (source.EndsWith(".png") || source.EndsWith(".jpg") || source.EndsWith(".bmp") || source.EndsWith("gif"));
        }

        bool CheckImage(string Path)
        {
            if (HasImageExtension(Path))
            {
                //Is Image
                var Name = GetFilename(Path);
                var Imag = SKImage.FromBitmap(SKBitmap.Decode(GetStream(Path)));
                mImageFunction(Name, Imag);
            }
            return true;
        }

        public ImageLoader(ProcessImage pLoaderFunction)
        {
            mImageFunction = pLoaderFunction;
            FileFunction = CheckImage;
        }
    }

    class XAMLLoader : ResourceLoader
    {
        public delegate bool ProcessXAML(string Name, string Xaml);
        private ProcessXAML mImageFunction;
        private bool HasXamlExtension(string source)
        {
            return source.EndsWith(".xaml");
        }

        bool CheckXaml(string Path)
        {
            if (HasXamlExtension(Path))
            {
                //Is an XAML file
                var Name = GetFilename(Path);

            }
            return true;
        }

        public XAMLLoader(ProcessXAML pLoaderFunction)
        {
            mImageFunction = pLoaderFunction;
            FileFunction = CheckXaml;
        }
    }
}
