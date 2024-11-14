using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;

namespace RealEstateSentimentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SentimentController : Controller
    {
        private static MLContext _mlContext = new MLContext();
        private static ITransformer _model = BuildAndTrainModel(_mlContext);

        private readonly PredictionEngine<SentimentData, SentimentPrediction> _predictionEngine;

        public SentimentController(PredictionEngine<SentimentData, SentimentPrediction> predictionEngine)
        {
            _predictionEngine = predictionEngine;
        }

        // POST api/sentiment/analyze
        [HttpPost("analyze")]
        public IActionResult AnalyzeSentiment([FromBody] SentimentData input)
        {
            if (input == null || string.IsNullOrEmpty(input.SentimentText))
            {
                return BadRequest("Text is required.");
            }

            var predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
            var prediction = predictionEngine.Predict(input);
            float roundedProbability = (float)Math.Round(prediction.Probability, 1);

            return Ok(new { Score = roundedProbability });
        }

        static ITransformer BuildAndTrainModel(MLContext mlContext)
        {
            
            var dataView = mlContext.Data.LoadFromTextFile<SentimentData>("data/yelp_labelled.txt", separatorChar: '\t');
            var splitData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.3);

            var estimator = mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.SentimentText))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression("Label", "Features"));

            var model = estimator.Fit(splitData.TrainSet);
            return model;
        }
    }
}
