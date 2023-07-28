namespace NeuralNetwork_Test_cSharp.DTO
{
    public class SimulationParameters
    {
        public int SimulationId { get; set; }

        public Dictionary<int, (int min, int max)> SpaceDimensions { get; set; }        

        public int PopulationNumber { get; set; }
        public int UnitLifeSpan { get; set; }

        public SelectionShapeEnum SelectionShape { get; set; }

        // Circular constraints
        public float Radius { get; set; }
        public float xCenter { get; set; }
        public float yCenter { get; set; }

        // Rectangle Constraints
        public float RecXmin { get; set; }
        public float RecXmax { get; set; }
        public float RecYmin { get; set; }
        public float RecYmax { get; set; }
    }
}
