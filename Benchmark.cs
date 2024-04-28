using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class Benchmark
    {
        public const string pathOut = @"C:\Temp";

        public string name;
        public List<int> sizes;
        public int trialsPerSize;
        public int? maxSeconds;
        public bool writeFile;
        public string? outFileName;
        public SortedDictionary<string, BenchmarkVariation> variations = new();

        public Benchmark(string name, List<int> sizes, int trialsPerSize, int? maxSeconds = null, bool writeFile = false)
        {
            this.name = name;
            this.sizes = sizes;
            this.trialsPerSize = trialsPerSize;
            this.maxSeconds = maxSeconds;
            this.writeFile = writeFile;

            if (writeFile)
            {
                this.outFileName = GetOutFileName();
            }
        }

        public void RecordTrial(string variationLabel, string? variationValue, int size, TimeSpan setupTime, TimeSpan runTime)
        {
            FindOrAddVariation(variationLabel, variationValue)
                .FindOrAddSize(size)
                .AddTrial(setupTime, runTime);
        }

        private BenchmarkVariation FindOrAddVariation(string variationLabel, string? variationValue)
        {
            string key = BenchmarkVariation.MakeKey(variationLabel, variationValue);
            if (!this.variations.ContainsKey(key))
            {
                this.variations.Add(key, new BenchmarkVariation(this, variationLabel, variationValue));
            }
            return this.variations[key];
        }

        public void Print()
        {
            PrintHeader();
            foreach (BenchmarkVariation variation in this.variations.Values)
            {
                variation.PrintHeader();
                foreach (BenchmarkSize benchmarkSize in variation.sizes.Values)
                {
                    benchmarkSize.PrintLine();
                }
            }
        }

        public void PrintHeader()
        {
            WriteLine($"\nBenchmark: \"{this.name}\":");
        }

        /*
        public void RunTrials(Action<int> action, string label, int minSize, int maxSize, int sizeMult, int trialsPerSize, int? maxSeconds = null)
        {
            PrintLabelHeader(label);
            int size = minSize;
            Stopwatch stOverall = Stopwatch.StartNew();
            // while (size <= maxSize && (maxSeconds == null || stOverall.Elapsed.TotalSeconds < maxSeconds))
            while (size <= maxSize)
            {
                double trialSeconds = 0.0;

                for (int i = 0; i < trialsPerSize; i++)
                {
                    Stopwatch st = Stopwatch.StartNew();
                    action(size);
                    TimeSpan timeSpan = st.Elapsed;
                    RecordTrial(label, size, timeSpan);
                    trialSeconds = timeSpan.TotalSeconds;

                    if (maxSeconds != null && i < (trialsPerSize - 1))
                    {
                        double secondsSoFar = stOverall.Elapsed.TotalSeconds;
                        double projectedSeconds = secondsSoFar + trialSeconds;
                        if (projectedSeconds > maxSeconds)
                        {
                            break;
                        }
                    }
                }

                PrintSize(label, size);

                if (maxSeconds != null)
                {
                    double secondsSoFar = stOverall.Elapsed.TotalSeconds;
                    double projectedSeconds = secondsSoFar + (trialSeconds * sizeMult);
                    if (projectedSeconds > maxSeconds)
                    {
                        break;
                    }
                }

                size *= sizeMult;
            }
        }

        public static void RunAndPrint(string benchmarkName, Action<int> action, string label, int minSize, int maxSize, int sizeMult, int trialsPerSize, int? maxSeconds = null)
        {
            Benchmark benchmark = new(benchmarkName);
            benchmark.PrintBenchmarkHeader();
            benchmark.RunTrials(action, label, minSize, maxSize, sizeMult, trialsPerSize, maxSeconds);
            // benchmark.Print();
        }
        */

        public static Benchmark StartBenchmark(string name, List<int> sizes, int trialsPerSize, int? maxSeconds = null, bool writeFile = false)
        {
            Benchmark benchmark = new Benchmark(name, sizes, trialsPerSize, maxSeconds, writeFile);
            benchmark.PrintHeader();
            return benchmark;
        }

        public void RunVariation<T>(Func<int, T> setup, Action<T> action, string variationLabel, string? variationValue)
        {
            string variationKey = BenchmarkVariation.MakeKey(variationLabel, variationValue);

            BenchmarkVariation variation = this.FindOrAddVariation(variationLabel, variationValue);
            variation.PrintHeader();

            Stopwatch stOverall = Stopwatch.StartNew();

            double trialSeconds = 0.0;

            for (int sizeIndex = 0; sizeIndex < this.sizes.Count; sizeIndex++)
            {
                int size = this.sizes[sizeIndex];

                if (this.maxSeconds != null && sizeIndex > 0)
                {
                    // Before starting trials for this size, estimate whether the first trial
                    // will go over the max seconds.
                    double secondsSoFar = stOverall.Elapsed.TotalSeconds;
                    // Assume that the time for this trial will be proportional to the size compared
                    // to the last trial for the previous size.
                    double projectedSecondsFirstTrial = trialSeconds * (size / this.sizes[sizeIndex - 1]);
                    double projectedSeconds = secondsSoFar + projectedSecondsFirstTrial;
                    if (projectedSeconds > this.maxSeconds)
                    {
                        break;
                    }
                }

                BenchmarkSize benchmarkSize = variation.FindOrAddSize(size);

                for (int i = 0; i < this.trialsPerSize; i++)
                {
                    Stopwatch st = Stopwatch.StartNew();
                    T input = setup(size);
                    TimeSpan setupTime = st.Elapsed;

                    st.Restart();
                    action(input);
                    TimeSpan runTime = st.Elapsed;
                    trialSeconds = runTime.TotalSeconds;

                    benchmarkSize.AddTrial(setupTime, runTime);

                    if (this.maxSeconds != null && i < (this.trialsPerSize - 1))
                    {
                        // Don't do another trial for this size if it looks like we'll go
                        // over the max seconds.
                        double secondsSoFar = stOverall.Elapsed.TotalSeconds;
                        double projectedSeconds = secondsSoFar + trialSeconds;
                        if (projectedSeconds > this.maxSeconds)
                        {
                            break;
                        }
                    }
                }

                benchmarkSize.PrintLine();

                if (this.writeFile)
                {
                    benchmarkSize.WriteFileRow();
                }

            }
        }

        public string GetOutFileName()
        {
            string fileName = Util.Format.CleanFileName($"Benchmark {this.name} {this.sizes.Min()}-{this.sizes.Max()} {trialsPerSize} {maxSeconds.GetValueOrDefault(0)} {DateTime.Now.ToString("s")}.txt");
            return Path.Combine(pathOut, fileName);
        }

    }

    public class BenchmarkVariation
    {
        public Benchmark benchmark;
        public string variationLabel;
        public string? variationValue;
        public SortedDictionary<int, BenchmarkSize> sizes = new();

        public BenchmarkVariation(Benchmark benchmark, string variationLabel, string? variationValue)
        {
            this.benchmark = benchmark;
            this.variationLabel = variationLabel;
            this.variationValue = variationValue;
        }

        public BenchmarkSize FindOrAddSize(int size)
        {
            if (!this.sizes.ContainsKey(size))
            {
                this.sizes.Add(size, new BenchmarkSize(this, size));
            }
            return this.sizes[size];
        }

        internal static string MakeKey(string variationLabel, string? variationValue)
        {
            string varValue = variationValue ?? string.Empty;
            return $"{variationLabel} {varValue}".Trim();
        }

        public string Key
        {
            get
            {
                return MakeKey(this.variationLabel, this.variationValue);
            }
        }

        public void PrintHeader()
        {
            WriteLine($"\t\"{this.Key}\":");
        }
    }

    public class BenchmarkSize
    {
        public BenchmarkVariation variation;
        public int size;
        public List<BenchmarkTrial> trials = new();

        public BenchmarkSize(BenchmarkVariation variation, int size)
        {
            this.variation = variation;
            this.size = size;
        }

        public void AddTrial(TimeSpan setupTime, TimeSpan runTime)
        {
            this.trials.Add(new BenchmarkTrial(this, setupTime, runTime));
        }

        public int TrialCount
        {
            get
            {
                return this.trials.Count;
            }
        }

        public double MinSetupMsec
        {
            get
            {
                return this.trials.Min(trial => trial.setupTime.TotalMilliseconds);
            }
        }

        public double MeanSetupMsec
        {
            get
            {
                return this.trials.Average(trial => trial.setupTime.TotalMilliseconds);
            }
        }

        public double MaxSetupMsec
        {
            get
            {
                return this.trials.Max(trial => trial.setupTime.TotalMilliseconds);
            }
        }
        public double MinRunMsec
        {
            get
            {
                return this.trials.Min(trial => trial.runTime.TotalMilliseconds);
            }
        }

        public double MeanRunMsec
        {
            get
            {
                return this.trials.Average(trial => trial.runTime.TotalMilliseconds);
            }
        }

        public double MaxRunMsec
        {
            get
            {
                return this.trials.Max(trial => trial.runTime.TotalMilliseconds);
            }
        }

        public void PrintLine()
        {
            WriteLine($"\t\t\"{this.size:n}\" trials = {this.TrialCount}; mean setup = {this.MeanSetupMsec}; min run = {this.MinRunMsec}; "
                + $"mean run = {this.MeanRunMsec}; max run = {this.MaxRunMsec}");
        }

        public void WriteFileRow()
        {
            Benchmark benchmark = this.variation.benchmark;
            File.AppendAllText(benchmark.outFileName!, $"{benchmark.name}\t{this.variation.variationLabel}\t{this.variation.variationValue}\t{this.size}\t{this.TrialCount}\t"
                        + $"{this.MinRunMsec}\t{this.MeanRunMsec}\t{this.MaxRunMsec}\t"
                        + $"{this.MinSetupMsec}\t{this.MeanSetupMsec}\t{this.MaxSetupMsec}\n");
        }
    }

    public class BenchmarkTrial
    {
        public BenchmarkSize benchmarkSize;
        public TimeSpan setupTime;
        public TimeSpan runTime;

        internal BenchmarkTrial(BenchmarkSize benchmarkSize, TimeSpan setupTime, TimeSpan runTime)
        {
            this.benchmarkSize = benchmarkSize;
            this.setupTime = setupTime;
            this.runTime = runTime;
        }
    }
}
