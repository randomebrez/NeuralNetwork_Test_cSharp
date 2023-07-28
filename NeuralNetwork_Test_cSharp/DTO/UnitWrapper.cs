using NeuralNetwork.Abstraction.Model;

namespace NeuralNetwork_Test_cSharp.DTO
{
    public class UnitWrapper
    {
        public UnitWrapper()
        {
            XPos = new List<float>();
            YPos = new List<float>();
        }

        public Guid Identifier { get; private set; } = Guid.NewGuid();
        public int SimulationId { get; set; }
        public int GenerationId { get; set; }
        public Unit Unit { get; set; }
        public float Score { get; set; }

        public List<float> XPos { get; set; }
        public List<float> YPos { get; set; }
    }
}
