using Microsoft.ML;
using Microsoft.ML.Data;
using System.Globalization;
using System.Text;
using static Microsoft.ML.DataOperationsCatalog;

namespace Predictor
{
    public class Program
    {
        // Define the path to your data file
        static string _yelpDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "rated_sentences(3).txt");

        static MLContext _mlContext = new MLContext();

        public static void Main()
        {
            TrainTestData splitDataView = LoadData(_mlContext);

            ITransformer model = BuildAndTrainModel(_mlContext, splitDataView.TrainSet);

            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            MakePredictionsOnTestSet(_mlContext, model, splitDataView.TestSet);

            EvaluateModel(_mlContext, model, splitDataView.TestSet);

            SaveModel(_mlContext, model, splitDataView.TrainSet.Schema, "sentiment_model.zip");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static TrainTestData LoadData(MLContext mlContext)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_yelpDataPath, separatorChar: '\t');
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.3); // 30% for testing, 70% for training
            return splitDataView;
        }

        static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            var estimator = mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.SentimentText))
            .Append(mlContext.Transforms.Conversion.MapValueToKey("Label"))
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = estimator.Fit(splitTrainSet);
            return model;
        }

        static void MakePredictionsOnTestSet(MLContext mlContext, ITransformer model, IDataView testSet)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

            var testSentences = mlContext.Data.CreateEnumerable<SentimentData>(testSet, reuseRowObject: false);
            foreach (var sentence in testSentences)
            {
                var prediction = predictionEngine.Predict(sentence);

                Console.WriteLine($"{sentence.SentimentText}");
                Console.WriteLine($"Score: {prediction.Predict}");
                Console.WriteLine();
            }
        }

        static void SaveModel(MLContext mlContext, ITransformer model, DataViewSchema schema, string modelPath)
        {
            mlContext.Model.Save(model, schema, modelPath);
        }

        static void EvaluateModel(MLContext mlContext, ITransformer model, IDataView testSet)
        {
            IDataView predictions = model.Transform(testSet);

            var metrics = mlContext.MulticlassClassification.Evaluate(predictions);

            Console.WriteLine($"Log-loss: {metrics.LogLoss}");
            Console.WriteLine($"Macro accuracy: {metrics.MacroAccuracy}");
            Console.WriteLine($"Micro accuracy: {metrics.MicroAccuracy}");



        }
    }
}
