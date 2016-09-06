using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLayerPerceptron
{
    class Program
    {
        static void Main(string[] args)

        {
            /*FileStream fsrMLP = new FileStream("XOR MLP.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader srMLP = new StreamReader(fsrMLP);
            FileStream fswMLP = new FileStream("XOR MLP.txt", FileMode.Append, FileAccess.Write, FileShare.Read);
            StreamWriter swMLP = new StreamWriter(fswMLP); //TO SOLVE can I have a sr and then a sw on the same doc without closing it at first ?*/

            FileStream fsMLPUpdated = new FileStream("Finance MLP updated.txt", FileMode.OpenOrCreate);
            //FileStream fsMLPUpdated = new FileStream("MLPUpdated XOR.txt", FileMode.OpenOrCreate);
            StreamWriter swMLPUpdated = new StreamWriter(fsMLPUpdated);

            FileStream fsData = new FileStream("data.txt", FileMode.Open, FileAccess.Read, FileShare.None);//raw data extracted from ukforex.co.uk
            StreamReader srData = new StreamReader(fsData); // data not organized in trainingSets yet

            FileStream fsTrainingSets = new FileStream("TrainingSets Created Finance.txt", FileMode.OpenOrCreate);
            //FileStream fsTrainingSets = new FileStream("trainingSets XOR.txt", FileMode.Open);
            //StreamReader srTrainingSets = new StreamReader(fsTrainingSets);
            StreamWriter swTrainingSets = new StreamWriter(fsTrainingSets);

            FileStream fsError = new FileStream("errorsFinance500to1000.txt", FileMode.OpenOrCreate);
            //FileStream fsError = new FileStream("errors XOR.txt", FileMode.OpenOrCreate);
            StreamWriter swError = new StreamWriter(fsError);

            FileStream fsResults = new FileStream("results.csv", FileMode.Append);          
            
            try
            {
                MLP mlp = new MLP();

                int[] dimensionLayers = new int[] { 3, 2, 1 }; //ISSUE too big, not all training sets are written in txt file // no it's not actually the issue, it's just because there are the training sets from all the tests//nope actually it is the issue :(

                mlp.InitializeMLPWithRandomWeights(dimensionLayers);
                //mlp.LoadMLP(srMLP);

                for (int i = 0; i < 500; i++)
                    srData.ReadLine();

                int nbTrainingSets = 50;
                mlp.createTrainingSetsFromFile(srData, swTrainingSets, nbTrainingSets, dimensionLayers[0], dimensionLayers[dimensionLayers.Length - 1]);
                //mlp.LoadTrainingSets(srTrainingSets);
                
                Console.WriteLine("Beginning of learning for mlp");

                double learningRate = 0.3d;
                double errorThreshold = 0.01d;
                int maxLearn = 20000;

                mlp.learn(learningRate, errorThreshold, maxLearn, swError);

                mlp.writeWeightsInFile(swMLPUpdated);

                Console.WriteLine("End of learning for mlp - Press any key to exit");
            }
            
            catch(MLPException e)
            {
                Console.WriteLine(e.Message);
            }

            catch(IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            catch(FormatException e)
            {
                Console.WriteLine(e.Message);
            }

            finally
            {
                /*if (srMLP != null)
                    srMLP.Close();

                if (fsrMLP != null)
                    fsrMLP.Close();*/

                if (fsMLPUpdated != null)
                    fsMLPUpdated.Close();

               /* if (fswMLP != null)
                    fswMLP.Close();*/

                if (fsData != null)
                    fsData.Close();
                if (srData != null)
                    srData.Close();

                if (fsError != null)
                    fsError.Close();

                if (fsTrainingSets != null)
                    fsTrainingSets.Close();

                if (fsResults != null)
                    fsResults.Close();
            }
            Console.ReadKey();             
        }
    }
}
