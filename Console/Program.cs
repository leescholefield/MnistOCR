using Core;
using System;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {

        private static readonly string TEST_SET_LOC = "C:/Users/leesc/source/repos/OCR/Core/Dataset/TestModels/mnist_test.csv";
        private static readonly string TRAINED_DATA_DIR = "C:/Users/leesc/source/repos/OCR/Core/Dataset/TrainedModels";

        static void Main(string[] args)
        {
            Task t = MainAsync(args);
            t.Wait();
        }

        static async Task MainAsync(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                System.Console.WriteLine("No args supplied. Exiting");
                return;
            }

            if (args[0] == "--train")
            {
                var trainer = new MnistTrainer();
                System.Console.WriteLine("Starting training");
                trainer.Train("C:/Users/leesc/source/repos/OCR/Core/Dataset/TestModels/mnist_train.csv", "");
            }
            else if (args[0] == "--test")
            {
                System.Console.WriteLine("Starting testing");
                var tester = new MnistTester(new MnistTester.Configuration { KNumber = 3, TrainedSamples = 100 });
                var result = await tester.Test(TEST_SET_LOC, TRAINED_DATA_DIR);

                System.Console.WriteLine("Duration: " + result.Duration);
                System.Console.WriteLine("Correct: " + result.NumCorrectGuesses);
                System.Console.WriteLine("Incorrect: " + result.NumIncorrectGuesses);
                System.Console.WriteLine("Incorrect by label:");
                foreach (var i in result.IncorrectGuessesForLabel)
                {
                    System.Console.WriteLine(String.Format("---{0}: {1}", i.Key, i.Value));
                }
            }
        }
    }
}
