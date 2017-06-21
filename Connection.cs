using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLayerPerceptron
{
    class Connection
    {
        Neuron prevNrn;
        Neuron nextNrn;
        double weight;
        double deltaWeight;
        double gradient;

        internal Neuron PrevNrn
        {
            get { return prevNrn; }
            set { prevNrn = value; }
        }        

        internal Neuron NextNrn
        {
            get { return nextNrn; }
            set { nextNrn = value; }
        }

        public double Weight
        {
            get { return weight; }
            set { weight = value; }
        }
        
        public double DeltaWeight
        {
            get { return deltaWeight; }
            set { deltaWeight = value; }
        }

        public double Gradient
        {
            get { return gradient; }

            set { gradient = value; }
        }

        public Connection(Neuron prev, Neuron next, double w)
        {
            //if one layer is null create it before this test
            //can a nrn exist without layer ? 
            if (prev.getCurrentLayer().getNextLayer().Equals(next.getCurrentLayer()))
            {
                this.prevNrn = prev;
                this.nextNrn = next;
                this.weight = w;
                this.deltaWeight = 0;
                //Console.WriteLine(" New connection from nrn " + prevNrn.getIndexInLayer() + " layer " + prevNrn.getCurrentLayer().IndexInMLP + " to nrn " + nextNrn.getIndexInLayer() + " layer " + nextNrn.getCurrentLayer().IndexInMLP + " weight " + weight);
            }
            else
                throw new MLPException("Connection ctor - the layers aren't previous-next to each other");
        }

        public Boolean isNextNrn(Neuron n)
        {
            if (this.nextNrn.Equals(n))
                return true;
            else
                return false;
        }

        public Boolean isPrevNrn(Neuron n)
        {
            if (this.prevNrn.Equals(n))
                return true;
            else
                return false;
        }
    }
}
