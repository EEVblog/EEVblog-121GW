using System.Reflection;
using System.IO;
using SkiaSharp;
using System.Xml;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml.Internals;
using System.Xml.Serialization;

namespace rMultiplatform
{
	class ResourceLoader
	{
		public delegate bool ProcessFile(string Filename);

		private	 ProcessFile _FileFunction;
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


	class GeneralLoader : ResourceLoader
	{
		public delegate bool	ProcessItem(string Name, Stream Stream);

		private List<string>	mExtensions;
		protected ProcessItem   mGeneralFunction;

		protected virtual bool  HasExtension(string source)
		{
			foreach (var ext in mExtensions)
				if (source.EndsWith(ext))
					return true;
			return false;
		}
		bool					Run(string Path)
		{
			if (HasExtension(Path))
			{
				//Is Image
				var Name = GetFilename(Path);
				mGeneralFunction(Name, GetStream(Path));
			}
			return true;
		}
		public				  GeneralLoader(ProcessItem pLoaderFunction, string pExtension)
		{
			var stringSeparators = new string[] { "," };
			mExtensions = new List<string>(pExtension.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries));

			mGeneralFunction	= pLoaderFunction;
			FileFunction		= Run;
		}
	}

	class ImageLoader : ResourceLoader
	{
		public delegate bool ProcessImage(string Name, SKImage Image);
		private ProcessImage mImageFunction;
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
	class SVGLoader : ResourceLoader
	{
		public delegate bool ProcessImage(string Name, SKSvg Image);
		private ProcessImage mImageFunction;

		private bool HasSvgExtension(string source)
		{
			return source.EndsWith(".svg");
		}
		bool CheckSvg(string Path)
		{
			if (HasSvgExtension(Path))
			{
				//Is Image
				var Name = GetFilename(Path);
				var Imag = new SKSvg();
				Imag.Load(GetStream(Path));
				mImageFunction(Name, Imag);
			}
			return true;
		}
		public SVGLoader(ProcessImage pLoaderFunction)
		{
			mImageFunction = pLoaderFunction;
			FileFunction = CheckSvg;
		}
	}
}
