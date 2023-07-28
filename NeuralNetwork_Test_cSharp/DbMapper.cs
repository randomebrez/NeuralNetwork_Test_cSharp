using NeuralNetwork_Test_cSharp.DTO;
using NeuralNetwork_Test_cSharp.DTO.DatabaseModel;
using System.Text;

namespace NeuralNetwork_Test_cSharp
{
    public static class DbMapper
    {
        public static SimulationDb ToDb(SimulationParameters simulation)
        {
            var simulationDb = new SimulationDb
            {
                EnvironmentLimits = $"{simulation.Xmin}:{simulation.Xmax}:{simulation.Ymin}:{simulation.Ymax}",
                SelectionShape = ToDb(simulation.SelectionShape),
                UnitLifeSpan = simulation.UnitLifeSpan
            };
            switch (simulation.SelectionShape)
            {
                case SelectionShapeEnum.Circular:
                    simulationDb.SelectionConstraints = $"{simulation.xCenter}:{simulation.yCenter}:{simulation.Radius}";
                    break;
                case SelectionShapeEnum.Rectangle:
                    simulationDb.SelectionConstraints = $"{simulation.RecXmin}:{simulation.RecXmax}:{simulation.RecYmin}:{simulation.RecYmax}";
                    break;
                default:
                    throw new Exception("Wtf is this");
            }
            return simulationDb;
        }

        public static UnitDb ToDb(UnitWrapper unit)
        {
            return new UnitDb
            {
                Identifier = unit.Identifier.ToString(),
                ParentA = unit.Unit.ParentA.ToString(),
                ParentB = unit.Unit.ParentB.ToString(),
                GenerationId = unit.GenerationId,
                SimulationId = unit.SimulationId,
                SelectionScore = unit.Score
            };
        }

        public static List<UnitStepDb> ToDb(string unitIdentifier, List<float> xPos, List<float> yPos)
        {
            var result = new List<UnitStepDb>();
            var currentIndex = 0;
            while(currentIndex < xPos.Count)
            {
                var maxIndex = Math.Min(xPos.Count(), currentIndex + 50);
                var unitSteps = new UnitStepDb
                {
                    UnitIdentifier = unitIdentifier,
                    LifeSteps = $"{currentIndex}-{maxIndex}"
                };
                var positions = new StringBuilder();
                for (int i = currentIndex; i < maxIndex; i++)
                    positions.Append($"{xPos[i]}:{yPos[i]}!");

                unitSteps.Positions = positions.ToString();
                result.Add(unitSteps);

                currentIndex = maxIndex;
            }
            
            return result;
        }

        public static string ToDb(SelectionShapeEnum selectionShape)
        {
            switch(selectionShape)
            {
                case SelectionShapeEnum.Circular:
                    return "Circular";
                case SelectionShapeEnum.Rectangle:
                    return "Rectangle";
                default:
                    throw new Exception("Wtf is this");
            }
        }
    }
}
