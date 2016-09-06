using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MultiLayerPerceptron
{
    class FileManager
    {
        /*FileStream fs;
        MLP mlp;

        public FileManager(FileStream f)
        {
            fs = f;
            mlp = new MLP();
        }

        public MLP LoadMLP()
        {
            Console.WriteLine("Loading of the MLP begins");
            StreamReader sr = new StreamReader(fs);
            Random r = new Random();

            //no need of doing the 3 lines underneath, already done in ctor
            //mlp.setLayers(new List<Layer>());
            //mlp.setGradEx(new List<double[]>());
            //mlp.setNbLayers(0);

            String[] lines = new string[fs.Length];
            int nbLines = 0;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                lines[nbLines] = line;
                Console.WriteLine(line);
                nbLines++;
            }

            try
            {
                mlp.setNbLayers(Convert.ToInt32(lines[0]));
                mlp.setLayers(new List<Layer>(mlp.getNbLayers()));
                //Console.WriteLine("there are " + mlp.getNbLayers() + " layers"); //index doesn't correspond to any Layer => we haven't created the layers !
                mlp.setLayerSizes(new int[mlp.getNbLayers() + 1]);
                mlp.setLayerSize(0, 0); //this does not generate exception
                Console.WriteLine("Layer 0 " + mlp.getLayer(0).ToString()); //here getLayer says layer 0 doesn't exist
                //mlp.setLayer(0, new Layer(0, 0, r)); //setLayer reacts as if index of Layer in mlp was null, index OOR
                Console.WriteLine("toto");
                //underneath, getLayerSize not happpy bc layer is null and index 00R 1 for size collection 0
                //we need to create these new layers somewhere
                for (int j = 0; j <= mlp.getNbLayers(); j++) //le problème vient d'ici  //used to be j=1//now with j=0 index OOR but doesn't generate exceptions ! grumpf.
                {
                    mlp.setLayerSize(j,Convert.ToInt32(lines[j])); // the 1st actual layer is layer 1 and not 0, we have the first slot being 0 as the previous of the 1st layer                       
                    Console.WriteLine("totototototo"); //this one is printed
                    mlp.addLayer(new Layer(mlp.getLayerSize(j), mlp.getLayerSize(j - 1), r)); // issue : we need to know the nb of nrn in the previous layer ; solution : the previous layer is defined as the one above in the file
                    Console.WriteLine("Layer " + j + " has been added and it has " + mlp.getLayerSize(j) + " neurons");
                }

                // Now we want to read each line specifying the connections between neurons and their weights
                for (int k = mlp.getNbLayers() + 1; k < nbLines; k++)
                {
                    StringReader s = new StringReader(lines[k]);
                    int lineLength = lines[k].Length;
                    char[] c = new char[lineLength];
                    s.Read(c, 0, lineLength);

                    int layerNumber = Convert.ToInt32(c[0].ToString());
                    if (layerNumber > mlp.getNbLayers()) //mlp.getnboflayer = layers.count
                        throw new MLPException("this layer number is superior to the layers size");

                    int nrnNumber = Convert.ToInt32(c[2].ToString());
                    if (nrnNumber > mlp.getLayer(layerNumber).size())
                        throw new MLPException("this nrn number is superior to the neurons size for this layer ");

                    int prevLayerNumber = Convert.ToInt32(c[4].ToString());
                    if (prevLayerNumber != layerNumber - 1) // we check that it's actually the previous of layerNumber
                        throw new MLPException("the 2nd layer is not the previous layer of the 1st one");

                    int prevNrnNumber = Convert.ToInt32(c[6].ToString());
                    if (prevNrnNumber > mlp.getLayer(prevLayerNumber).size())
                        throw new MLPException("this nrn number is superior to the neurons size for the previous layer");

                    double weight;
                    StringBuilder sb = new StringBuilder(lines[k]); //this way we read as many digits of weight as there are
                    sb.Remove(0, 7);
                    bool parsing = double.TryParse(sb.ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out weight);
                    if (!parsing)
                        throw new MLPException("Parsing of weight failed");

                    //mlp.setWeight(layerNumber, nrnNumber, prevNrnNumber, weight);
                    mlp.addConnection(prevNrnNumber, nrnNumber, prevLayerNumber, weight);
                }

                mlp.setGradEx(new List<double[]>());
                for (int i = 0; i < mlp.getNbLayers(); i++)
                {
                    mlp.addToGradEx(new double[mlp.getLayerSize(i)]);
                }

                Console.WriteLine("Loading of the MLP finished");
                return this.mlp;
            }

            catch (FormatException e)
            {
                throw new FormatException("Input string is not a sequence of digits.", e);
            }

            finally
            {
                sr.Close(); // is it going to be executed ? :s we are throwing a new exception in the above catch ... 
            }

        }

        //it'd be nice to also have a method LoadExamples but this would require knowing its format (or passing it as parameter)*/
    }
}
