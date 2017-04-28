using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLayerPerceptron
{
        class Layer  
        {
            List<Neuron> Neurons;
            Layer prevLayer;
            Layer nextLayer;
            MLPType layerType;
            int indexInMLP;

            internal MLPType LayerType
            {
                get
                {
                    return layerType;
                }

                set
                {
                    layerType = value;
                }
            }

            public int IndexInMLP
            {
                get
                {
                    return indexInMLP;
                }

                set
                {
                    indexInMLP = value;
                }
            }

            public Layer(Layer l)
            {
                layerType = l.layerType;
                Neurons = new List<Neuron>(l.getNeurons());
                prevLayer = l.getPrevLayer(); //passed by reference on purpose
                nextLayer = l.getNextLayer(); //passed by reference on purpose                
            }

            public Layer(int nbNrn)
            {
                //this.layerType = lType; //we don't put it bc this ctor is only used for the false layer which is the prev of the input layer and it doesn't have any type
                Neurons = new List<Neuron>();
                Neuron tmp = null;
                int i = 0;
                for (i = 0; i < nbNrn; i++)
                {
                    tmp = new Neuron(i, this, this.layerType);
                    //Console.WriteLine("current layer set for neuron " + i + " in layer " + this.ToString());
                    Neurons.Add(tmp);
                }
            }

            public Layer(int nbNrn, Layer prev, MLPType lType, int index)
            {
                layerType = lType;
                prevLayer = prev;
                IndexInMLP = index;
                Neurons = new List<Neuron>();
                Neuron tmp = null;
                int i = 0;
                for (i = 0; i < nbNrn; i++)
                {
                    tmp = new Neuron(i, this, layerType);
                    //Console.WriteLine("current layer set for neuron " + i + " in layer " + this.ToString());
                    Neurons.Add(tmp);
                }
            }

            public int size() { return this.Neurons.Count; } 

            public Neuron getNeuron(int i) { return this.Neurons[i]; }
            public List<Neuron> getNeurons() { return this.Neurons; }        
            public int getNbOfPrevNeur() { return this.prevLayer.size(); }
            public Layer getPrevLayer() { return this.prevLayer; }
            public void setPrevLayer(Layer l) { this.prevLayer = l; } //should be done only in ctor, setter to delete ultimately

            public Layer getNextLayer() { return this.nextLayer; }
            public void setNextLayer(Layer l) { this.nextLayer = l; } 
                
            /*public void addBias()
            {
                Neuron n = new Neuron(0, this, MLPType.bias);
                Neurons.Add(n);
            }*/
    }
}
