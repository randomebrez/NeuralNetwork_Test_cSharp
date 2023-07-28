using NeuralNetwork_Test_cSharp.DTO;
using NeuralNetwork_Test_cSharp.DTO.DatabaseModel;
using System.Text;

namespace NeuralNetwork_Test_cSharp
{
    public static class DbMapper
    {
        public static SimulationDb ToDb(SimulationParameters simulation)
        {
            var envLimits = new StringBuilder();
            foreach (var dimension in simulation.SpaceDimensions)
                envLimits.Append($"{dimension.Value.min}:{dimension.Value.max}:");

            var simulationDb = new SimulationDb
            {
                EnvironmentLimits = envLimits.ToString(),
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
            var currentStepIndex = 0;
            while(currentStepIndex < xPos.Count)
            {
                var maxStepIndex = Math.Min(xPos.Count, currentStepIndex + 50);
                var unitSteps = new UnitStepDb
                {
                    UnitIdentifier = unitIdentifier,
                    LifeSteps = $"{currentStepIndex}-{maxStepIndex}"
                };

                var positionsDbValue = new StringBuilder();
                for (int i = currentStepIndex; i < maxStepIndex; i++)
                    positionsDbValue.Append($"{xPos[i]}:{yPos[i]}!");

                unitSteps.Positions = positionsDbValue.ToString();
                result.Add(unitSteps);

                currentStepIndex = maxStepIndex;
            }
            
            return result;
        }

        public static string ToDb(SelectionShapeEnum selectionShape)
        {
            return selectionShape switch
            {
                SelectionShapeEnum.Circular => "Circular",
                SelectionShapeEnum.Rectangle => "Rectangle",
                _ => throw new Exception("Wtf is this"),
            };
        }
    }
}
