using Microsoft.ML.Data;

namespace RealEstateSentimentAPI
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string? SentimentText { get; set; }

        [LoadColumn(1)]
        public bool Label { get; set; }
    }
}
