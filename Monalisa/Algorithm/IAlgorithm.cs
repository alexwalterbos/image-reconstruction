using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.monalisa.algorithm
{
    public interface IAlgorithm
    {
        Task RunAsync();
        event EventHandler EpochCompleted;
        event EventHandler AlgorithmCompleted;
    }
}
