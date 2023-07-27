using NeuralNetwork_Test_cSharp.DTO;
using NeuralNetwork_Test_cSharp.DTO.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            };
            switch (simulation.SelectionShape)
            {
                case SelectionShapeEnum.Circular:
                    simulationDb.SelectionConstraints = $"{simulation.Center}:{simulation.Radius}";
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
                GenerationId = unit.GenerationId,
                SimulationId = unit.SimulationId,
                SelectionScore = unit.Score
            };
        }

        public static List<UnitStepDb> ToDb(string unitIdentifier, List<float> xPos, List<float> yPos)
        {
            var result = new List<UnitStepDb>();
            for(int i = 0; i < xPos.Count(); i++)
            {
                result.Add(new UnitStepDb
                {
                    UnitIdentifier = unitIdentifier,
                    LifeStepId = i,
                    Position = $"{xPos[i]}:{yPos[i]}"
                });
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
