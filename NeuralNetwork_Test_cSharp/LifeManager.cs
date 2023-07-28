using NeuralNetwork.Abstraction;
using NeuralNetwork.Abstraction.Model;
using NeuralNetwork.Implementations;
using NeuralNetwork_Test_cSharp.DTO;
using System.Text;

namespace NeuralNetwork_Test_cSharp
{
    public class LifeManager
    {
        private IGenomeManager _genomeManager;
        private IBrainCalculator _brainCalculator;

        private SimulationParameters _simulationParameters;
        private List<string> _brainNames = new List<string> { "Main" };

        private BrainCaracteristics _caracteristic;

        public int GenerationId;
        public UnitWrapper[] Units;

        public LifeManager(SimulationParameters parameters)
        {
            _simulationParameters = parameters;
            
            Units = new UnitWrapper[parameters.PopulationNumber];
            _genomeManager = new GenomeManager();
            _brainCalculator = new BrainCalculator();
        }


        // Initialisation
        public List<UnitWrapper> InitialyzeUnits()
        {
            GenerationId = 0;
            _caracteristic = new BrainCaracteristics
            {
                IsDecisionBrain = true,
                BrainName = "Main",
                InputLayer = new LayerCaracteristics(0, LayerTypeEnum.Input)
                {
                    NeuronNumber = 4,
                    ActivationFunction = ActivationFunctionEnum.Identity,
                    ActivationFunction90PercentTreshold = 0,
                    NeuronTreshold = 0
                },
                NeutralLayers = new List<LayerCaracteristics> { new LayerCaracteristics(1, LayerTypeEnum.Neutral){
                    NeuronNumber = 2,
                    ActivationFunction = ActivationFunctionEnum.Tanh,
                    ActivationFunction90PercentTreshold = 1f,
                }, },
                OutputLayer = new LayerCaracteristics(2, LayerTypeEnum.Output)
                {
                    NeuronNumber = 4,
                    ActivationFunction = ActivationFunctionEnum.Sigmoid,
                    ActivationFunction90PercentTreshold = 1f,
                    NeuronTreshold = 0f,
                },
                GenomeCaracteristics = new GenomeCaracteristics
                {
                    GeneNumber = 50,
                    WeighBytesNumber = 4
                }
            };

            var units = GenerateRandomUnits(_simulationParameters.PopulationNumber);

            for (int i = 0; i < _simulationParameters.PopulationNumber; i++)
                Units[i] = units[i];

            return Units.ToList();
        }

        private List<UnitWrapper> GenerateRandomUnits(int unitNumber)
        {
            var result = new List<UnitWrapper>();

            var genomes = _genomeManager.GenomesListGet(unitNumber, _caracteristic).Select(t => new GenomeWrapper { Genome = t });
            var graphs = GenomeGraphGet(genomes.ToList());

            var units = _genomeManager.UnitsFromGenomeGraphList(graphs);

            for (int i = 0; i < unitNumber; i++)
                result.Add(new UnitWrapper
                {
                    Unit = units[i],
                    XPos = new List<float> { new Random().Next(_simulationParameters.Xmin, _simulationParameters.Xmax) },
                    YPos = new List<float> { new Random().Next(_simulationParameters.Ymin, _simulationParameters.Ymax) },
                    SimulationId = _simulationParameters.SimulationId,
                    GenerationId = GenerationId
                }) ;

            return result;
        }

        private List<GenomeGraph> GenomeGraphGet(List<GenomeWrapper> genomes)
        {
            var graphs = new List<GenomeGraph>();
            foreach (var genome in genomes)
                graphs.Add(new GenomeGraph
                {
                    ParentA = genome.ParentA,
                    ParentB = genome.ParentB,
                    GenomeNodes = new List<GenomeCaracteristicPair>
                    {
                        new GenomeCaracteristicPair
                        {
                            Caracteristics = _caracteristic,
                            Genome = genome.Genome
                        }
                    }
                });
            return graphs;
        }


        // Generation's Life Execution
        public void ExecuteGenerationLife()
        {
            for (int i = 0; i < _simulationParameters.UnitLifeSpan; i++)
                ExecuteLifeStep();
            Console.WriteLine();
        }

        private void ExecuteLifeStep()
        {
            for (int j = 0; j < _simulationParameters.PopulationNumber; j++)
            {
                var currentUnit = Units[j];
                var inputs = new Dictionary<string, List<float>>
                {
                    { "Main", GetInputs(currentUnit) }
                };

                _brainCalculator.BrainGraphCompute(currentUnit.Unit.BrainGraph, inputs);

                //if (j == 0)
                //{
                //    var message = new StringBuilder("Inputs : ");
                //    foreach(var input in inputs["Main"])
                //        message.Append($"{input.ToString()},");
                //
                //    Console.WriteLine(message.ToString());
                //}

                MoveUnit(currentUnit, false);
            }
        }

        private List<float> GetInputs(UnitWrapper unit)
        {
            var currentXpos = unit.XPos[^1];
            var currentYpos = unit.YPos[^1];
            return new List<float>
            {
                1 - (_simulationParameters.Xmax - currentXpos)/(2*_simulationParameters.Xmax),
                1 - (currentXpos - _simulationParameters.Xmin)/(2*_simulationParameters.Xmax),
                1 - (_simulationParameters.Ymax - currentYpos)/(2*_simulationParameters.Ymax),
                1 - (currentYpos - _simulationParameters.Ymin)/(2*_simulationParameters.Ymax)
            };

        }

        private void MoveUnit(UnitWrapper unit, bool withMessage)
        {
            var decisionBrain = unit.Unit.BrainGraph.DecisionBrain;
            var outputs = decisionBrain.Neurons.OutputLayer.Neurons.ToList();
            var message = new StringBuilder("Outputs : ");
            (int bestOutputIndex, float bestOutputValue) = (-1, 0);
            for (int i = 0; i < outputs.Count(); i++)
            {
                if (outputs[i].Value > bestOutputValue)
                    (bestOutputIndex, bestOutputValue) = (outputs[i].Id, outputs[i].Value);
                if (withMessage)
                    message.Append($"{outputs[i].Value},");
            }

            if (withMessage)
                Console.WriteLine(message.ToString());
            var currentXpos = unit.XPos[^1];
            var currentYpos = unit.YPos[^1];

            switch (bestOutputIndex)
            {
                case -1:
                    break;
                case 0:
                    //Down
                    if (currentYpos - 1 >= _simulationParameters.Ymin)
                    {
                        unit.YPos.Add(currentYpos - 1);
                        unit.XPos.Add(currentXpos);
                    }                        
                    else
                    {
                        unit.YPos.Add(currentYpos + 1);
                        unit.XPos.Add(currentXpos);
                    }
                    break;
                case 1:
                    //Up
                    if (currentYpos + 1 <= _simulationParameters.Ymax)
                    {
                        unit.YPos.Add(currentYpos + 1);
                        unit.XPos.Add(currentXpos);
                    }
                    else
                    {
                        unit.YPos.Add(currentYpos - 1);
                        unit.XPos.Add(currentXpos);
                    }
                    break;
                case 2:
                    //Right
                    if (currentXpos + 1 <= _simulationParameters.Xmax)
                    {
                        unit.XPos.Add(currentXpos + 1);
                        unit.YPos.Add(currentYpos);
                    }
                    else
                    {
                        unit.XPos.Add(currentXpos - 1);
                        unit.YPos.Add(currentYpos);
                    }
                    break;
                case 3:
                    //Left
                    if (currentXpos - 1 >= _simulationParameters.Xmin)
                    {
                        unit.XPos.Add(currentXpos - 1);
                        unit.YPos.Add(currentYpos);
                    }
                    else
                    {
                        unit.XPos.Add(currentXpos + 1);
                        unit.YPos.Add(currentYpos);
                    }
                    break;
                case 4:
                    RandomMove(unit);
                    break;
                case 5:
                    unit.XPos.Add(currentXpos);
                    unit.YPos.Add(currentYpos);
                    break;
            }
        }

        private void RandomMove(UnitWrapper unit)
        {
            var availablePositions = new List<string>();
            var currentXpos = unit.XPos[^1];
            var currentYpos = unit.YPos[^1];
            if (currentYpos - 1 >= _simulationParameters.Ymin)
                availablePositions.Add("Down");
            if (currentYpos + 1 <= _simulationParameters.Ymax)
                availablePositions.Add("Up");
            if (currentXpos + 1 <= _simulationParameters.Xmax)
                availablePositions.Add("Right");
            if (currentXpos - 1 >= _simulationParameters.Xmin)
                availablePositions.Add("Left");

            var index = new Random().Next(availablePositions.Count);
            switch(availablePositions[index])
            {
                case ("Down"):
                    unit.YPos.Add(currentYpos - 1);
                    unit.XPos.Add(currentXpos);
                    break;
                case ("Up"):
                    unit.YPos.Add(currentYpos + 1);
                    unit.XPos.Add(currentXpos);
                    break;
                case ("Right"):
                    unit.XPos.Add(currentXpos + 1);
                    unit.YPos.Add(currentYpos);
                    break;
                case ("Left"):
                    unit.XPos.Add(currentXpos - 1);
                    unit.YPos.Add(currentYpos);
                    break;
            }
        }


        // Reproduction
        public int ReproduceUnits()
        {
            GenerationId++;

            //Select best units
            var bestUnits = SelectBestUnits(0.4f);

            var mixedGenomes = GetMixedGenomes(bestUnits);
            // Tres bizarre, il faut un graphe a suivre pour reconstruire à partir du dico de <brainName, List<Genome>>
            var genomes = mixedGenomes.ContainsKey("Main") ? mixedGenomes["Main"] : new List<GenomeWrapper>();
            var graphs = GenomeGraphGet(genomes);
            var childrenUnits = _genomeManager.UnitsFromGenomeGraphList(graphs);
            var remainingUnitToGenerate = _simulationParameters.PopulationNumber - childrenUnits.Length;

            var randomChildren = GenerateRandomUnits(remainingUnitToGenerate);

            for(int i = 0; i < childrenUnits.Length; i++)
            {
                Units[i] = new UnitWrapper
                {
                    Unit = childrenUnits[i],
                    XPos = new List<float> { new Random().Next(_simulationParameters.Xmin, _simulationParameters.Xmax) },
                    YPos = new List<float> { new Random().Next(_simulationParameters.Ymin, _simulationParameters.Ymax) },
                    SimulationId = _simulationParameters.SimulationId,
                    GenerationId = GenerationId
                };
            }
            for(int i = 0; i < randomChildren.Count; i++)
            {
                Units[i + childrenUnits.Length] = randomChildren[i];
            }
            return remainingUnitToGenerate;
        }

        private Dictionary<string, List<GenomeWrapper>> GetMixedGenomes(List<UnitWrapper> bestUnits)
        {
            var result = new Dictionary<string, List<GenomeWrapper>>();
            var couples = CreateCouples(bestUnits);

            foreach (var couple in couples)
            {
                foreach (var brainName in _brainNames)
                {
                    var genomeA = couple.parentA.Unit.BrainGraph.BrainNodes[brainName].Genome;
                    var genomeB = couple.parentB.Unit.BrainGraph.BrainNodes[brainName].Genome;
                    var mixedGenome = _genomeManager.GenomeCrossOverGet(genomeA, genomeB, _caracteristic, 1, 0.001f);

                    if (result.TryGetValue(brainName, out var genomes))
                        genomes.Add(new GenomeWrapper
                        {
                            Genome = mixedGenome,
                            ParentA = couple.parentA.Unit.Identifier,
                            ParentB = couple.parentB.Unit.Identifier,
                        });
                    else
                        result.Add(brainName, new List<GenomeWrapper> { new GenomeWrapper
                        {
                            Genome = mixedGenome,
                            ParentA = couple.parentA.Unit.Identifier,
                            ParentB = couple.parentB.Unit.Identifier,
                        } });
                }
            }

            return result;
        }

        private (UnitWrapper parentA, UnitWrapper parentB)[] CreateCouples(List<UnitWrapper> bestUnits)
        {
            var result = new List<(UnitWrapper parentA, UnitWrapper parentB)>();
            while (bestUnits.Count > 1 && result.Count < _simulationParameters.PopulationNumber)
            {
                var firstindex = new Random().Next(bestUnits.Count);
                var parentA = bestUnits[firstindex];
                var secondIndex = firstindex;
                while (secondIndex == firstindex)
                    secondIndex = new Random().Next(0, bestUnits.Count);
                var parentB = bestUnits[secondIndex];
                result.Add((parentA, parentB));
                parentA.Unit.ChildrenNumber++;
                parentB.Unit.ChildrenNumber++;
                bestUnits = bestUnits.Where(t => t.Unit.ChildrenNumber < t.Unit.MaxChildNumber).ToList();
            }

            return result.ToArray();
        }


        //Selection
        public float UnitScoreAssign()
        {
            switch (_simulationParameters.SelectionShape)
            {
                case SelectionShapeEnum.Circular:
                    return CircularScoreAssign();
                case SelectionShapeEnum.Rectangle:
                    return RectangleScoreAssign();
                default:
                    return 0;
            }
        }

        public int SurvivorNumberCount()
        {
            switch (_simulationParameters.SelectionShape)
            {
                case SelectionShapeEnum.Circular:
                    return CircularSurvivorNumberCount();
                case SelectionShapeEnum.Rectangle:
                    return RectangleSurvivorNumberCount();
                default:
                    return 0;
            }
        }


        private float CircularScoreAssign()
        {
            var meanScore = 0f;
            var xCenter = _simulationParameters.xCenter;
            var yCenter = _simulationParameters.yCenter;
            for (int i = 0; i < _simulationParameters.PopulationNumber; i++)
            {
                var currentXpos = Units[i].XPos[^1];
                var currentYpos = Units[i].YPos[^1];

                var centerDist = (currentXpos - xCenter) * (currentXpos - xCenter) + (currentYpos - yCenter) * (currentYpos - yCenter);
                Units[i].Score = centerDist;

                meanScore += centerDist;
            }

            return meanScore / _simulationParameters.PopulationNumber;
        }

        private int CircularSurvivorNumberCount()
        {
            return Units.Where(t => t.Score < _simulationParameters.Radius * _simulationParameters.Radius).Count();
        }


        private float RectangleScoreAssign()
        {
            var meanScore = 0f;
            for (int i = 0; i < _simulationParameters.PopulationNumber; i++)
            {
                var currentXpos = Units[i].XPos[^1];
                var currentYpos = Units[i].YPos[^1];

                var unitScore = Math.Abs((currentXpos - _simulationParameters.RecXmin) - (_simulationParameters.RecXmax - currentXpos)) + Math.Abs((currentYpos - _simulationParameters.RecYmin) - (_simulationParameters.RecYmax - currentYpos));
                Units[i].Score = unitScore;

                meanScore += unitScore;
            }

            return meanScore / _simulationParameters.PopulationNumber;
        }

        private int RectangleSurvivorNumberCount()
        {
            var survivorNumber = 0;
            for (int i = 0; i < _simulationParameters.PopulationNumber; i++)
            {
                var currentXpos = Units[i].XPos[^1];
                var currentYpos = Units[i].YPos[^1];
                if (currentXpos >= _simulationParameters.RecXmin && currentXpos <= _simulationParameters.RecXmax && currentYpos >= _simulationParameters.RecYmin && currentYpos <= _simulationParameters.RecYmax)
                    survivorNumber++;
            }

            return survivorNumber;
        }


        private List<UnitWrapper> SelectBestUnits(float percentageToSelect, bool sortScoreDesc = false)
        {
            var orderedUnits = sortScoreDesc ? Units.OrderByDescending(t => t.Score) : Units.OrderBy(t => t.Score);
            var survivorNumber = SurvivorNumberCount();
            var maxToTake = Math.Max(survivorNumber, percentageToSelect * _simulationParameters.PopulationNumber);
            var selected = orderedUnits.Take((int)maxToTake).ToList();

            return selected;
        }
    }
}
