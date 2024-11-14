using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictor
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string? SentimentText;

        [LoadColumn(1), ColumnName("Label")]
        public uint Sentiment;
    }

    public class SentimentPrediction : SentimentData
    {
        [ColumnName("PredictedLabel")]
        public uint Predict { get; set; }
        public float[] Score { get; set; }

    }
}
