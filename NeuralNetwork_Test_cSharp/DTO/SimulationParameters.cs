namespace NeuralNetwork_Test_cSharp.DTO
{
    public class SimulationParameters
    {
        public int SimulationId { get; set; }

        public int Xmin { get; set; }
        public int Xmax { get; set; }

        public int Ymin { get; set; }
        public int Ymax { get; set; }

        public int PopulationNumber { get; set; }
        public int UnitLifeSpan { get; set; }

        public SelectionShapeEnum SelectionShape { get; set; }

        // Circular constraints
        public float Radius { get; set; }
        public float Center { get; set; }

        // Rectangle Constraints
        public float RecXmin { get; set; }
        public float RecXmax { get; set; }
        public float RecYmin { get; set; }
        public float RecYmax { get; set; }
    }
}
