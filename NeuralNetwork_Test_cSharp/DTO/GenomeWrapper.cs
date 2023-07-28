using NeuralNetwork.Abstraction.Model;

namespace NeuralNetwork_Test_cSharp.DTO
{
    public class GenomeWrapper
    {
        public Genome Genome { get; set; }

        public Guid ParentA { get; set; }

        public Guid ParentB { get; set; }
    }
}
