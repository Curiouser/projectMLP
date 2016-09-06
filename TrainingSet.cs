using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiLayerPerceptron
{
    class TrainingSet
    {
        double[] input;
        double[] output;

        public TrainingSet()
        {
        }

        public TrainingSet(int dimInput, int dimOutput)
        {
            input = new double[dimInput];
            output = new double[dimOutput];
        }

        /*public TrainingSet(double[] inp, double[] outp)
        {
            //this.input = inp;
            //this.output = outp;
            inp.CopyTo(input, 0);
            outp.CopyTo(output, 0);
        }*/

        public double[] Input
        {
            get
            {
                return input;
            }

            set
            {
                input = value;
            }
        }

        public double[] Output
        {
            get
            {
                return output;
            }

            set
            {
                output = value;
            }
        }       
    }
}
