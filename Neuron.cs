using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLayerPerceptron
{
    class Neuron 
    {            
        MLPType nrnType;
        public void setNrnType(MLPType n) { nrnType = n; }
        public MLPType getNrnType() { return this.nrnType; }

        double rightVal = 0; 
        double leftVal = 0; 
        double actDeriv = 0;
        double error = 0;
        double delta = 0;

        public double ActDeriv
        {
            get { return actDeriv; }
            set { actDeriv = value; }
        }

        public double LeftVal
        {
            get
            {
                return leftVal;
            }

            set
            {
                leftVal = value;
            }
        }

        public double RightVal
        {
            get
            {
                return rightVal;
            }

            set
            {
                rightVal = value;
            }
        }

        public double Error
        {
            get
            {
                return error;
            }

            set
            {
                error = value;
            }
        }

        public double Delta
        {
            get
            {
                return delta;
            }

            set
            {
                delta = value;
            }
        }

        //float lambda = 1.5f; // parameter of the sigmoid
        Layer currentLayer; 
        int IndexInLayer;

        public Neuron(int index, Layer curLayer, MLPType neuronType)
        {
            nrnType = neuronType;
            IndexInLayer = index;
            currentLayer = curLayer; // passed by reference on purpose
            if (neuronType == MLPType.bias)
                rightVal = 1.0d;
        }

        public Neuron(Neuron n)
        {
            nrnType = n.getNrnType();
            rightVal = n.RightVal;
            leftVal = n.LeftVal;
            currentLayer = n.getCurrentLayer();
            IndexInLayer = n.getIndexInLayer();
        }

        public double getActivation() { return rightVal; }
        public Layer getCurrentLayer() { return currentLayer; }
        public int getIndexInLayer() { return IndexInLayer; }
        
        private void computeActivDerivative()
        {
            double explmx = Math.Exp(- leftVal); //TO CHECK right or left ?
            actDeriv =  explmx / ((1 + explmx) * (1 + explmx)); 
        }

        public void activateNeuron()
        {
            rightVal = 1.0f / (1.0f + Math.Exp(-leftVal)); 
            computeActivDerivative();
        }
    }
}
