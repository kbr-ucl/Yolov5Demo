using System.Drawing;
using Yolov5Net.Scorer;
using Yolov5Net.Scorer.Models;

// https://github.com/mentalstack/yolov5-net

var assetsRelativePath = @"../../../Assets";
var assetsPath = GetAbsolutePath(assetsRelativePath);
var modelFilePath = Path.Combine(assetsPath, "Model", "yolov5s.onnx");
var imagesFolder = Path.Combine(assetsPath, "Images");
var outputFolder = Path.Combine(assetsPath, "Images", "Output");

using var image = Image.FromFile($"{imagesFolder}/image7.jpg");

using var scorer = new YoloScorer<YoloCocoP5Model>(modelFilePath);

List<YoloPrediction> predictions = scorer.Predict(image);
var labels = predictions.Select(a => a.Label.Name).Distinct().ToList();
var persons = predictions.Where(a => a.Label.Name.ToLower().Equals("person")).ToList();
using var graphics = Graphics.FromImage(image);

foreach (var prediction in persons) // iterate predictions to draw results
{
    double score = Math.Round(prediction.Score, 2);

    graphics.DrawRectangles(new Pen(prediction.Label.Color, 1),
        new[] { prediction.Rectangle });

    var (x, y) = (prediction.Rectangle.X - 3, prediction.Rectangle.Y - 23);

    graphics.DrawString($"{prediction.Label.Name} ({score})",
        new Font("Arial", 16, GraphicsUnit.Pixel), new SolidBrush(prediction.Label.Color),
        new PointF(x, y));
}

image.Save($"{outputFolder}/result.jpg");

Console.WriteLine("========= End of Process..Hit any Key ========");

string GetAbsolutePath(string relativePath)
{
    var _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
    var assemblyFolderPath = _dataRoot.Directory.FullName;

    var fullPath = Path.Combine(assemblyFolderPath, relativePath);

    return fullPath;
}