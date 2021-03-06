﻿using SiaNet.Common;
using SiaNet.Model;
using SiaNet.Model.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiaNet.Examples
{
    internal class HousingRegression
    {
        private static TrainTestFrame traintest;

        private static Sequential model;

        public static void LoadData()
        {
            DataFrame frame = new DataFrame();
            Downloader.DownloadSample(SampleDataset.HousingRegression);
            var samplePath = Downloader.GetSamplePath(SampleDataset.HousingRegression);
            frame.LoadFromCsv(samplePath.Train);
            var xy = frame.SplitXY(14, new[] { 1, 13 });
            traintest = xy.SplitTrainTest(0.25);
        }

        public static void BuildModel()
        {
            model = new Sequential();
            model.Add(new Dense(13, 12, OptActivations.ReLU));
            model.Add(new Dense(13, OptActivations.ReLU));
            model.Add(new Dense(1));

            model.OnEpochEnd += Model_OnEpochEnd;
            model.OnTrainingEnd += Model_OnTrainingEnd;
        }

        public static void Train()
        {
            model.Compile(OptOptimizers.Adam, OptLosses.MeanSquaredError, OptMetrics.MAE, Regulizers.RegL2(0.01));
            model.Train(traintest.Train, 500, 32, traintest.Test);
        }

        private static void Model_OnTrainingEnd(Dictionary<string, List<double>> trainingResult)
        {
            var mean = trainingResult[OptMetrics.MAE].Mean();
            var std = trainingResult[OptMetrics.MAE].Std();
            Console.WriteLine("Training completed. Mean: {0}, Std: {1}", mean, std);
        }

        private static void Model_OnEpochEnd(int epoch, uint samplesSeen, double loss, Dictionary<string, double> metrics)
        {
            Console.WriteLine(string.Format("Epoch: {0}, Loss: {1}, Accuracy: {2}", epoch, loss, metrics["val_mae"]));
        }
    }
}
