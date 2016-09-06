using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLayerPerceptron
{
    enum MLPType { input, hidden, output, bias }

    class MLP
    {
        List<Layer> Layers;
        List<Connection> Connections;
        List<TrainingSet> TrainingSets; 
        double networkError = double.PositiveInfinity; 

        public double NetworkError
        {
            get
            {
                return networkError;
            }

            set
            {
                networkError = value;
            }
        }
        
        public MLP()
        {
            Layers = new List<Layer>();
            Connections = new List<Connection>();
            TrainingSets = new List<TrainingSet>();
        }

        public void LoadMLP(StreamReader sr)
        {
            string line;
            int nbLayers = 0;
            int result;
            int i = 0, j = 0, nNeurons = 0;
            bool parsing;
            double w = 0.0d;
            int[] vals = new int[4];

            while ((line = sr.ReadLine()) != null)
            {
                //determine the number of layers
                if (i == 0)
                    {
                    parsing = Int32.TryParse(line, out result);
                    if (parsing)
                        nbLayers = result;
                    else
                        throw new FormatException("File Format Incorrect");

                    //create the layers
                    for (j = 0; j < nbLayers; j++)
                    {
                        line = sr.ReadLine();
                        parsing = Int32.TryParse(line, out result);
                        if (parsing)
                            nNeurons = result;
                        else
                            throw new FormatException("File Format Incorrect");

                        if (j == 0)
                            Layers.Add(new Layer(nNeurons, new Layer(0), MLPType.input, 0));
                        else if (j == nbLayers - 1)
                            Layers.Add(new Layer(nNeurons, Layers[j - 1], MLPType.output, nbLayers - 1));
                        else 
                            Layers.Add(new Layer(nNeurons, Layers[j - 1], MLPType.hidden, j));

                    }

                }

                else if (i == 1)
                {
                    int k = 0;
                    //set up layers
                    if (nbLayers == 1)
                    {
                        Layers[0].setNextLayer(null);
                        Layers[0].setPrevLayer(null);
                    }

                    else if (nbLayers == 2)
                    {
                        Layers[0].setPrevLayer(null);
                        Layers[0].setNextLayer(Layers[1]);
                        Layers[1].setNextLayer(null);
                        Layers[1].setPrevLayer(Layers[0]);
                    }

                    else
                    {
                        Layers[0].setPrevLayer(null);
                        Layers[0].setNextLayer(Layers[1]);
                        Layers[Layers.Count - 1].setNextLayer(null);
                        Layers[Layers.Count - 1].setPrevLayer(Layers[Layers.Count - 2]);

                        for (k = 1; k < Layers.Count - 1; k++)
                        {
                            Layers[k].setPrevLayer(Layers[k - 1]);
                            Layers[k].setNextLayer(Layers[k + 1]);
                        }
                    }
                } 

                else if (i > 1)//set up the connections
                {
                    ParseConnection(line, vals, ref w);
                    Connection newcon = new Connection(Layers[vals[2]].getNeuron(vals[3]), Layers[vals[0]].getNeuron(vals[1]), w);

                    bool conIsNew = true;
                    foreach (Connection c in Connections)
                        if (c.NextNrn == newcon.NextNrn && c.PrevNrn == newcon.PrevNrn)
                            conIsNew = false;

                    if (conIsNew)
                        Connections.Add(newcon);
                    else
                        throw new MLPException("method MLP.loadMLP - A connection between these two neurons already exists");
                }

                Console.WriteLine(line);
                i++;
            }
        }

        private void ParseConnection(string line, int[] p1, ref double p2)
        {
            int len = line.Length;
            bool parsing;
            int result;
            double d;
            string tmp;
            int i = 0, j = 0, k = 0;
            while (i < len)
            {
                if (k < 4)
                {
                    j = line.IndexOf(" ", i);
                    tmp = line.Substring(i, j - i);
                    parsing = Int32.TryParse(tmp, out result);
                    if (parsing)
                        p1[k] = result;
                    else
                        throw new FormatException("File Format Incorrect");
                    i = j + 1;
                }
                else
                {
                    tmp = line.Substring(i, len - i);
                    i = len;
                    parsing = Double.TryParse(tmp, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out d);
                    if (parsing)
                        p2 = d;
                    else
                        throw new FormatException("File Format Incorrect");
                }
                k++;
            }
        }

        public void InitializeMLPWithRandomWeights(int [] dimLayers)
        {
            Layer layer = null;
            Connection connection = null;
            Random r = new Random();
            double weight;
            int k = 0;
            int nbLayers = dimLayers.Length;

            for (int i = 0; i < nbLayers; i++) //create the layers
            {
                if (i == 0)
                    layer = new Layer(dimLayers[i], null, MLPType.input, 0); //QUESTION : is there going to be nbLayers layers existing or just one bc passed by ref ?

                else if (i == nbLayers - 1)
                    layer = new Layer(dimLayers[i], Layers[i - 1], MLPType.output, i);
            
                else
                    layer = new Layer(dimLayers[i], Layers[i - 1], MLPType.hidden, i);

                Layers.Add(layer);            
            }

            foreach (Layer lay in Layers) //set nextLayer for each Layer created
            {
                if (lay.LayerType != MLPType.output)
                {
                    lay.setNextLayer(Layers[k + 1]); //INDEX OOR
                    k++;
                }                
            }

            foreach (Layer l in Layers) //create a connection for any set of two existing neurons //THIS DOESNT WOOOOOOOOOOOOOOOOOOOOOOOOOOORK
            {
                if (l.LayerType != MLPType.output)
                {
                    foreach (Neuron prevN in l.getNeurons())
                    {
                        foreach (Neuron nextN in l.getNextLayer().getNeurons())
                        {
                            weight = r.NextDouble();
                            connection = new Connection(prevN, nextN, weight);
                            Connections.Add(connection);
                        }//end of foreach for next layer
                    }//end of setting connections for this layer
                }//end of if where we check that we're not in output layer                    
            }//end of foreach layer
        }

        public void LoadTrainingSets(StreamReader sr) // we use this only when the file already exists, otherwise use createTrainingSetsFromFile which creates the file and fills the container in MLP
        {
            Console.WriteLine("LOADING TRAINING SETS BEGINS ");
            string line;
            bool parsing;
            int result = 0;
            double tmp = 0;
            int i = 0, j = 0;
            int nbTrainingSets = 0;
            int dimInput = 0, dimOutput = 0;
            double[] tmpArray;
            while ((line = sr.ReadLine()) != null)
            {
                //determine the number of training sets
                if (i == 0)
                {
                    parsing = Int32.TryParse(line, out result);
                    if (parsing)
                        nbTrainingSets = result;
                    else
                        throw new FormatException("File Format Incorrect");
                        
                //create the training sets
                TrainingSets = new List<TrainingSet>(nbTrainingSets);
                TrainingSet ts = null;
                int toto = 0;
                for (toto = 0; toto < nbTrainingSets; toto++)
                {
                    ts = new TrainingSet();
                    TrainingSets.Add(ts);
                }

                //fill the training sets
                for (j = 0; j < nbTrainingSets; j++)
                {
                        line = sr.ReadLine();
                        parsing = Int32.TryParse(line, out result);
                        if (parsing)
                        {
                            dimInput = result;
                            tmpArray = new double[dimInput];
                            TrainingSets[j].Input = new double[dimInput];

                            for (int k = 0; k < dimInput; k++)
                            {
                                line = sr.ReadLine();
                                parsing = Double.TryParse(line, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out tmp);
                                tmpArray[k] = tmp;
                            }
                            tmpArray.CopyTo(TrainingSets[j].Input, 0); 
                        }

                        else
                            throw new FormatException("File Format Incorrect");

                        line = sr.ReadLine();
                        parsing = Int32.TryParse(line, out result);
                        if (parsing)
                        {
                            dimOutput = result;
                            tmpArray = new double[dimOutput];
                            TrainingSets[j].Output = new double[dimOutput];

                            for (int k = 0; k < dimOutput; k++)
                            {
                                line = sr.ReadLine();
                                parsing = Double.TryParse(line, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out tmp); //I've added numberstyles and cultureinfo bc I thought it'd help (comma instead of point) but it doesn't seem so
                                tmpArray[k] = tmp;
                            }
                            tmpArray.CopyTo(TrainingSets[j].Output, 0);
                        }

                        else
                            throw new FormatException("File Format Incorrect");
                    }
                }
                Console.WriteLine(line);
                i++;
            }
            Console.WriteLine("LOADING TRAINING SETS FINISHED");
        }
        
        public void LoadFirstLayer(int trainingSet)
        {
            if (Layers[0].getNeurons().Count == TrainingSets[trainingSet].Input.Length)
            {
                int i = 0;
                foreach (Neuron n in Layers[0].getNeurons())
                {
                    n.LeftVal = TrainingSets[trainingSet].Input[i];
                    i++;
                }
            }

            else
                throw new MLPException("method MLP.LoadFirstLayer size of layer " + Layers[0].getNeurons().Count + " and inputs is " + TrainingSets[trainingSet].Input.Length);            
        }
        
        public void createTrainingSetsFromFile(StreamReader sr, StreamWriter sw, int nbTrainingSets, int dimInputLayer, int dimOutputLayer) 
        {
            string[] linesInput = new string [dimInputLayer];
            string[] linesOutput = new string [dimOutputLayer];
            int t = 0;
            bool parsing;
            double tmp;

            sw.WriteLine(nbTrainingSets);
            sr.ReadLine();

            while (t < nbTrainingSets)
            {
                if (sr.EndOfStream)
                    throw new MLPException("The file is too small for this number of training sets, please choose a number inferior to " + t);

                TrainingSet tr = new TrainingSet(dimInputLayer, dimOutputLayer);

                sw.WriteLine(dimInputLayer);
                for (int i = 0; i < dimInputLayer; i++)
                {
                    if (!sr.EndOfStream)
                    {
                        linesInput[i] = sr.ReadLine().Substring(12);
                        parsing = Double.TryParse(linesInput[i], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out tmp);
                        if (parsing)
                        {
                            tr.Input[i] = tmp;
                            sw.WriteLine(tr.Input[i]);
                        }
                        else
                            throw new MLPException("method createTrainingSetsFromFile, parsing failed for training set " + t);                        
                    }    
                    else
                        throw new MLPException("The file is too small for this number of training sets, please choose a number inferior to " + t);
                }

                sw.WriteLine(dimOutputLayer);
                for (int j = 0; j < dimOutputLayer; j++)
                {
                    if (!sr.EndOfStream)
                    {
                        linesOutput[j] = sr.ReadLine().Substring(12);
                        parsing = Double.TryParse(linesOutput[j], NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out tmp);
                        if (parsing)
                        {
                            tr.Output[j] = tmp;
                            sw.WriteLine(tr.Output[j]);;
                        }
                        else
                            throw new MLPException("method createTrainingSetsFromFile, parsing failed for training set " + t);
                    }                        
                    else
                        throw new MLPException("The file is too small for this number of training sets, please choose a number inferior to " + t);
                }

                TrainingSets.Add(tr);
                t++;
            }
        }

        public int getNbLayers() { return Layers.Count; }

        public Layer getLayer(int i)
        {
            if (i >= 0 && i < Layers.Count)
                return Layers[i];
            else
                throw new MLPException("this index " + i + " does not correspond to any layer - method MLP.getLayer");
        }
        
        public int getLayerSize(int layerNb)
        {
            if (layerNb >= 0 && layerNb <= this.Layers.Count)
                return this.Layers[layerNb].size(); 
            else
                throw new MLPException("method getLayerSize, index OOR " + layerNb + " nbLayers = " + this.Layers.Count); 
        }        

        public void setWeight(int layerNb, int nrnNumber, int prevNrnNumber, double weight)
        {
            foreach (Connection c in Connections)
            {
                if ( c.isNextNrn(Layers[layerNb].getNeuron(nrnNumber)) && c.isPrevNrn(Layers[layerNb - 1].getNeuron(prevNrnNumber)))
                c.Weight = weight;
            }
        }      
        
        public List<Connection> getNrnPrevCon(Neuron nrn) //return all the Connections where the param nrn appears as the NextNrn
        {
            List<Connection> nrnCon = new List<Connection>();
            foreach (Connection c in this.Connections)
            {
                if (c.isNextNrn(nrn))
                    nrnCon.Add(c);
            }
            return nrnCon;
        }

        public List<Connection> getNrnNextCon(Neuron nrn) //return all the Connections where the param nrn appears as the PrevNrn
        {
            List<Connection> nrnCon = new List<Connection>();
            foreach (Connection c in this.Connections)
            {
                if (c.isPrevNrn(nrn))
                    nrnCon.Add(c);
            }
            return nrnCon;
        }

        public double getWeight(Neuron prev, Neuron next)
        {
            foreach (Connection c in Connections) 
            {
                if (c.isNextNrn(next) && c.isPrevNrn(prev))
                    return c.Weight;
            }
            throw new MLPException("method MLP.getWeight - this connection doesn't exist");
        }

        public double getDeltaWeight(Neuron prev, Neuron next)
        {
            foreach (Connection c in Connections) 
            {
                if (c.isNextNrn(next) && c.isPrevNrn(prev))
                    return c.DeltaWeight;
            }
            throw new MLPException("method MLP.getWeight - this connection doesn't exist");
        }
                
        public void setDeltaWeight(Neuron prev, Neuron next, double delta)
        {
            foreach (Connection c in Connections) 
            {
                if (c.isNextNrn(next) && c.isPrevNrn(prev))
                    c.DeltaWeight = delta;
            }
            throw new MLPException("method MLP.getWeight - this connection doesn't exist");
        }

        public double getGradient(Neuron prev, Neuron next)
        {
            foreach (Connection c in Connections) 
            {
                if (c.isNextNrn(next) && c.isPrevNrn(prev))
                    return c.Gradient;
            }
            throw new MLPException("method MLP.getWeight - this connection doesn't exist");
        }

        public void setGradient(Neuron prev, Neuron next, double gradient) //TO DO there should be only one connection in Connections for a given (prev, next) => we should check this when adding them in loadMLP
        {
            foreach (Connection c in Connections) 
            {
                if (c.isNextNrn(next) && c.isPrevNrn(prev))
                    c.DeltaWeight = gradient;
            }
            throw new MLPException("method MLP.getWeight - this connection doesn't exist");
        }
               
        public void addLayer(Layer l) { this.Layers.Add(l); } // we only add a layer on the right, otherwise too difficult to adjust connections again
        public void addConnection(Connection c) { this.Connections.Add(c); }
        public void addConnection(Neuron prev, Neuron next, double w) { this.Connections.Add(new Connection(prev, next, w)); }
        public void addConnection(int prevNrn, int nextNrn, int prevLayerNb, double w) { this.Connections.Add(new Connection(this.Layers[prevLayerNb].getNeuron(prevNrn), this.Layers[prevLayerNb + 1].getNeuron(nextNrn), w)); }
        //public void addTrainingSet(TrainingSet t) { this.TrainingSets.Add(t); } //TO CHECK is it ok to pass by ref a training set ?

        private void evaluateLayer(Layer l) 
        {
            List<Connection> tmp = null;
            double result = 0;
            if (l.getPrevLayer() != null)                
            {
                foreach (Neuron n in l.getNeurons())
                {
                    result = 0;
                    tmp = getNrnPrevCon(n);

                    foreach (Connection c in tmp)
                    {
                        result += c.PrevNrn.RightVal * c.Weight;
                    }
                    n.LeftVal = result; 
                    n.activateNeuron();
                }
            }
        }

        public void evaluate() 
        {
            foreach (Layer l in Layers)
                evaluateLayer(l);
        }

        private void evalNrnError(Neuron n, int trainingSet) 
        {
            if (n.getNrnType().Equals(MLPType.output))
                 n.Error = n.RightVal - TrainingSets[trainingSet].Output[n.getIndexInLayer()]; //Index OOR here // Not anymore, mouhaha ! >:D
            else
                throw new MLPException("method MLP.evalNrnError - the neuron is not an output nrn"); 
        }

        public void evaluateError(int trainingSet) 
        {
            double error = 0.0d;
            foreach (Neuron n in Layers[Layers.Count - 1].getNeurons())
            {
                evalNrnError(n, trainingSet);
                error += n.Error;
            }
            NetworkError = error;            
        }

        private void computeNrnDelta()
        {
            double sum = 0.0d;
            for (int l = Layers.Count - 1; l >= 0; l--)
            {
                foreach (Neuron n in Layers[l].getNeurons())
                {
                    if (n.getNrnType().Equals(MLPType.output))
                        n.Delta = -n.Error * n.ActDeriv;
                    else if (n.getNrnType().Equals(MLPType.hidden))
                    {
                        foreach (Connection c in getNrnNextCon(n))
                        {
                            sum += c.Weight * c.NextNrn.Delta;
                        }
                        n.Delta = -n.ActDeriv * sum;

                        if (Math.Abs(n.Delta) < Math.Pow(10, -99))
                            Console.WriteLine("Math.Abs(neuron delta) < 10^-99 ie n.Delta = " + n.Delta);
                    }
                }
            }
        } 

        private void evalGradients(int trainingSet)
        {
            foreach (Connection c in Connections) //order doesn't matter here, it only mattered in eval weights delta            
                c.Gradient = c.PrevNrn.RightVal * c.NextNrn.RightVal;            
        }

        private void resetWeightsDelta()
        {           
            Connections.FindAll(x => x.NextNrn == Layers[0].getNeuron(0));
            foreach (Connection c in this.Connections)
                c.DeltaWeight = 0.0d;
        }

        private void evaluateWeightsDelta(double learningRate) 
        {
            foreach (Connection c in Connections)
                c.DeltaWeight = learningRate * c.Gradient * c.PrevNrn.Delta; // * momentum if there's one
        }

        private void updateWeights() 
        {
            foreach (Connection c in Connections)
                c.Weight += c.DeltaWeight;
        }

        public void writeWeightsInFile(StreamWriter sw) 
        {
            sw.WriteLine(getNbLayers());

            foreach (Layer l in Layers)
                sw.WriteLine(l.size());

            foreach (Connection c in Connections)
                sw.WriteLine(c.NextNrn.getCurrentLayer().IndexInMLP + " " + c.NextNrn.getIndexInLayer() + " " + c.PrevNrn.getCurrentLayer().IndexInMLP + " " + c.PrevNrn.getIndexInLayer() + " " + c.Weight);

            sw.Flush();             
        }

        public void writeErrorInFile(StreamWriter sw, int trainingSet)
        {
            sw.WriteLine("TrainingSet {0} \tExpectedOutput \tOutputFromMLP \t\tError", trainingSet);

            int i = 0;
            foreach (Neuron n in Layers[Layers.Count - 1].getNeurons())
            {
                sw.WriteLine("\t\t" + TrainingSets[trainingSet].Output[i] + "\t\t" + n.RightVal + "\t" + n.Error);
                i++;
            }
            sw.WriteLine();
            sw.Flush();
        }

        private void backProp(double learningRate,  StreamWriter swErrors) 
        {
            resetWeightsDelta();
            for (int trainingSet = 0; trainingSet < TrainingSets.Count; trainingSet++) 
            {
                LoadFirstLayer(trainingSet);
                evaluate();
                evaluateError(trainingSet); 
                computeNrnDelta();
                evalGradients(trainingSet);
                evaluateWeightsDelta(learningRate);
                writeErrorInFile(swErrors, trainingSet); 
            }
            updateWeights(); //TO DECIDE should we put it in the for loop ? if outside : batch backprop; if inside : online backprop
        }

        public void learn(double learningRate, double errorThreshold, int maxLearn, StreamWriter swErrors)
        {
            int i = 0;
            while (Math.Abs(NetworkError) > errorThreshold && i < maxLearn)
            {
                this.backProp(learningRate, swErrors);
                if (i % 100 == 0)
                    Console.WriteLine("learn while loop index " + i + " : Network Error is " + networkError);
                i++;
            }
        }
    }
}
