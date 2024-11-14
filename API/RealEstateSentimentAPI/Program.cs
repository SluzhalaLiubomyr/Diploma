
using Microsoft.ML;

namespace RealEstateSentimentAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            
            var mlContext = new MLContext();
            var modelPath = Path.Combine(builder.Environment.ContentRootPath, "Model", "sentiment_model.zip");
            var model = mlContext.Model.Load(modelPath, out var modelInputSchema);
            builder.Services.AddSingleton(model);
            builder.Services.AddSingleton(p =>
            {
                var predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
                return predictionEngine;
            });

            var app = builder.Build();

            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");


            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
