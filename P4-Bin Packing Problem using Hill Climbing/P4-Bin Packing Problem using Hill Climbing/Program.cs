using System;
using System.Collections.Generic;
using System.Linq;

namespace BinPackingRMHC
{
    class Program
    {
        const int BinCapacity = 130;
        static readonly List<double> Blocks = new List<double>
        {
            55.44, 15.12, 0.77, 3.1, 95.24, 89.59, 7.12, 58.77, 78.98,
            55.76, 30.34, 17.5, 44.05, 74.76, 28.4, 96.55, 87.78, 65.56,
            72.91, 39.74, 39.52, 38.72, 8.12, 32.31, 12.28, 52.15, 89.93,
            15.9, 3.37, 2.18
        };

        static void Main(string[] args)
        {
            var solution = GenerateRandomSolution(Blocks);
            var bestSolution = RandomMutationHillClimbing(solution);

            Console.WriteLine("Optimized Bin Packing Solution:");
            PrintSolution(bestSolution);
            Console.WriteLine($"Total Bins Used: {bestSolution.Count}");
        }

        static List<List<double>> GenerateRandomSolution(List<double> blocks)
        {
            var random = new Random();
            var shuffledBlocks = blocks.OrderBy(_ => random.Next()).ToList();
            var bins = new List<List<double>>();

            foreach (var block in shuffledBlocks)
            {
                bool placed = false;
                foreach (var bin in bins)
                {
                    if (bin.Sum() + block <= BinCapacity)
                    {
                        bin.Add(block);
                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    bins.Add(new List<double> { block });
                }
            }

            return bins;
        }

        static double CalculateFitness(List<List<double>> solution)
        {
            int numBins = solution.Count;
            double totalUnusedSpace = 0;

            foreach (var bin in solution)
            {
                double usedSpace = bin.Sum();
                totalUnusedSpace += BinCapacity - usedSpace;
            }

            double normalizedUnusedSpace = totalUnusedSpace / (numBins * BinCapacity);

            return numBins + normalizedUnusedSpace;
        }

        static List<List<double>> RandomMutationHillClimbing(List<List<double>> initialSolution)
        {
            var bestSolution = CloneSolution(initialSolution);
            var bestFitness = CalculateFitness(bestSolution);
            var random = new Random();

            for (int iteration = 0; iteration < 10000; iteration++)
            {
                var mutatedSolution = MutateSolution(bestSolution);
                var mutatedFitness = CalculateFitness(mutatedSolution);

                if (mutatedFitness < bestFitness)
                {
                    bestSolution = mutatedSolution;
                    bestFitness = mutatedFitness;
                }
            }

            return bestSolution;
        }

        static List<List<double>> MutateSolution(List<List<double>> solution)
        {
            var random = new Random();
            var newSolution = CloneSolution(solution);

            var nonEmptyBins = newSolution.Where(bin => bin.Any()).ToList();
            var sourceBin = nonEmptyBins[random.Next(nonEmptyBins.Count)];
            var block = sourceBin[random.Next(sourceBin.Count)];
            sourceBin.Remove(block);

            if (!sourceBin.Any())
            {
                newSolution.Remove(sourceBin);
            }

            bool placed = false;
            foreach (var bin in newSolution)
            {
                if (bin.Sum() + block <= BinCapacity)
                {
                    bin.Add(block);
                    placed = true;
                    break;
                }
            }

            if (!placed)
            {
                newSolution.Add(new List<double> { block });
            }

            return newSolution;
        }

        static List<List<double>> CloneSolution(List<List<double>> solution)
        {
            return solution.Select(bin => new List<double>(bin)).ToList();
        }

        static void PrintSolution(List<List<double>> solution)
        {
            for (int i = 0; i < solution.Count; i++)
            {
                Console.WriteLine($"Bin {i + 1}: [{string.Join(", ", solution[i])}] (Total: {solution[i].Sum()} cm)");
            }
        }
    }
}
