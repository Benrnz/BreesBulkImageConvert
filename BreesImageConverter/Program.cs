using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BreesImageConverter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Brees 2017 Bulk Image Converter.");

            string convertFrom = ".bmp";
            string sourceFolder = ParseSourceFolder(args);     // @"C:\Users\ben\Pictures\Frontier Developments\Elite Dangerous\Experiment";
            string outputFolder = ParseOutputFolder(args); // @"C:\Users\ben\Pictures\Frontier Developments\Elite Dangerous\Experiment";
            string suffix = ParseOutputType(args);         // ".jpg";

            if (string.IsNullOrWhiteSpace(suffix) || string.IsNullOrWhiteSpace(sourceFolder) || string.IsNullOrWhiteSpace(outputFolder))
            {
                return;
            }

            Console.WriteLine($"Source Folder: {sourceFolder}");
            Console.WriteLine($"Output Folder: {outputFolder}");
            Console.WriteLine($"Converting to: {suffix}");
            Console.WriteLine();

            if (string.IsNullOrWhiteSpace(sourceFolder))
            {
                throw new ArgumentNullException("sourceFolder"); // Shouldn't occur.
            }

            foreach (var sourceFile in Directory.GetFiles(sourceFolder, $"*{convertFrom}"))
            {
                using (var sourceImage = new Bitmap(File.Open(Path.Combine(sourceFolder, sourceFile), FileMode.Open)))
                {
                    Console.WriteLine(sourceFile);
                    SaveImage(sourceImage, sourceFile, outputFolder, suffix);
                }
            }
        }

        private static string ParseOutputType(string[] args)
        {
            if (args.GetLength(0) < 3)
            {
                BadArguments();
                ShowHelp();
            }

            string type = args[2].ToLowerInvariant();
            if (type == "jpg")
            {
                return ".jpg";
            }

            if (type == "png")
            {
                return ".png";
            }

            Console.WriteLine($"Output File type not supported: {type}");
            ShowHelp();
            return null;
        }

        private static string ParseOutputFolder(string[] args)
        {
            if (args.GetLength(0) < 2)
            {
                BadArguments();
                ShowHelp();
                return null;
            }
            int i = 1;
            var secondParam = args[1];
            if (!Directory.Exists(secondParam))
            {
                Console.WriteLine("Folder does not exist:");
                Console.WriteLine(secondParam);
            }
            return secondParam;
        }

        private static string ParseSourceFolder(string[] args)
        {
            if (args.GetLength(0) < 1)
            {
                BadArguments();
                ShowHelp();
                return null;
            }
            var firstParam = args[0];
            if (!Directory.Exists(firstParam))
            {
                Console.WriteLine("Folder does not exist:");
                Console.WriteLine(firstParam);
                return null;
            }
            return firstParam;
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Commandline syntax:");
            Console.WriteLine("BreesImageConverter.exe <SourceFolder> <OutputFolder> <jpg|png>");
        }

        private static void BadArguments()
        {
            Console.WriteLine();
            Console.WriteLine("! Bad arguments: There are not enough arguments to run the command.");
        }

        private static void SaveImage(Bitmap sourceImage, string sourceFileName, string outputFolder, string suffix)
        {
            var encoder = Encoder.Quality;
            ImageCodecInfo codecInfo;
            var encoderParameters = new EncoderParameters(1);

            if (suffix == ".png")
            {
                codecInfo = GetEncoder(ImageFormat.Png);
                var myEncoderParameter = new EncoderParameter(encoder, 100L);
                encoderParameters.Param[0] = myEncoderParameter;
            }
            else if (suffix == ".jpg")
            {
                codecInfo = GetEncoder(ImageFormat.Jpeg);
                var myEncoderParameter = new EncoderParameter(encoder, 100L);
                encoderParameters.Param[0] = myEncoderParameter;
            }
            else
            {
                throw new NotSupportedException("File format not supported: " + suffix);
            }

            string destinationFileName = Path.GetFileNameWithoutExtension(sourceFileName);
            string destinationPath = Path.Combine(outputFolder, destinationFileName + suffix);
            int index = 1;
            while (File.Exists(destinationPath))
            {
                destinationPath = Path.Combine(outputFolder, destinationFileName + index++ + suffix);
            }

            sourceImage.Save(destinationPath, codecInfo, encoderParameters);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}