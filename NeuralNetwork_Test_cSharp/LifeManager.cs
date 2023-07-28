using NeuralNetwork.Abstraction;
using NeuralNetwork.Abstraction.Model;
using NeuralNetwork.Implementations;
using NeuralNetwork_Test_cSharp.DTO;

namespace NeuralNetwork_Test_cSharp
{
    public class LifeManager
    {
        private readonly IGenomeManager _genomeManager;
        private readonly IBrainCalculator _brainCalculator;

        private SimulationParameters _simulationParameters;
        private BrainCaracteristics _brainCaracteristic;
        private List<string> _brainNames = new List<string> { "Main" };

        public int PopulationNumber => _simulationParameters.PopulationNumber;
        public int GenerationId;
        public UnitWrapper[] Units;

        public LifeManager(SimulationParameters parameters, BrainCaracteristics caracteristics)
        {
            _simulationParameters = parameters;
            
            Units = new UnitWrapper[parameters.PopulationNumber];
            _genomeManager = new GenomeManager();
            _brainCalculator = new BrainCalculator();

            _brainCaracteristic = caracteristics;
        }


        // Initialisation
        public List<UnitWrapper> InitialyzeUnits()
        {
            GenerationId = 0;
            

            var units = GenerateRandomUnits(_simulationParameters.PopulationNumber);

            for (int i = 0; i < _simulationParameters.PopulationNumber; i++)
                Units[i] = units[i];

            return Units.ToList();
        }
        private List<UnitWrapper> GenerateRandomUnits(int unitNumber)
        {
            var result = new List<UnitWrapper>();

            var genomes = _genomeManager.GenomesListGet(unitNumber, _brainCaracteristic).Select(t => new GenomeWrapper { Genome = t });
            var graphs = GenomeGraphGet(genomes.ToList());

            var units = _genomeManager.UnitsFromGenomeGraphList(graphs);

            for (int i = 0; i < unitNumber; i++)
                result.Add(UnitBuild(units[i]));

            return result;
        }
        private UnitWrapper UnitBuild(Unit unit)
        {
            var initialCoordinates = new List<float>();
            foreach (var dimension in _simulationParameters.SpaceDimensions)
                initialCoordinates.Add(new Random().Next(dimension.Value.min, dimension.Value.max));

            var newUnit = new UnitWrapper
            {
                Unit = unit,
                CurrentPosition = new SpacePosition(initialCoordinates.ToArray()),
                SimulationId = _simulationParameters.SimulationId,
                GenerationId = GenerationId
            };
            newUnit.XPos = new List<float> { newUnit.CurrentPosition.GetCoordinate(0) };
            newUnit.YPos = new List<float> { newUnit.CurrentPosition.GetCoordinate(1) };

            return newUnit;
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
                            Caracteristics = _brainCaracteristic,
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
        }
        private void ExecuteLifeStep()
        {
            for (int j = 0; j < _simulationParameters.PopulationNumber; j++)
            {
                var currentUnit = Units[j];

                UnitCompute(currentUnit);
                UnitMove(currentUnit);
            }
        }


        // Unit brain managment
        private void UnitCompute(UnitWrapper unit)
        {
            var inputs = new Dictionary<string, List<float>>
                {
                    { "Main", GetInputs(unit) }
                };

            _brainCalculator.BrainGraphCompute(unit.Unit.BrainGraph, inputs);
        }
        private List<float> GetInputs(UnitWrapper unit)
        {
            var result = new List<float>();
            foreach (var envLimits in _simulationParameters.SpaceDimensions)
            {
                var dimensionCoordinate = unit.CurrentPosition.GetCoordinate(envLimits.Key);
                var (min, max) = _simulationParameters.SpaceDimensions[envLimits.Key];
                var delta = max - min;
                result.Add(1 - (max - dimensionCoordinate) / delta);
                result.Add(1 - (dimensionCoordinate - min) / delta);
            }

            return result;

        }
        private void UnitOutputInterpret(UnitWrapper unit, int outputIndex)
        {
            switch (outputIndex)
            {
                case -1:
                    break;
                case 0:
                    //Right
                    Move(unit, 0, 1);
                    break;
                case 1:
                    //Left
                    Move(unit, 0, -1);
                    break;
                case 2:
                    //Down
                    Move(unit, 1, -1);
                    break;
                case 3:
                    //Up
                    Move(unit, 1, 1);
                    break;
                case 4:
                    RandomMove(unit);
                    break;
                case 5:
                    //No move
                    break;
            }
        }
        private (int index, float value) HighestOutputGet(Brain brain)
        {
            var outputs = brain.Neurons.OutputLayer.Neurons;

            (int highestOutputIndex, float outputValue) = (-1, 0);
            for (int i = 0; i < outputs.Count(); i++)
            {
                if (outputs[i].Value > outputValue)
                    (highestOutputIndex, outputValue) = (outputs[i].Id, outputs[i].Value);
            }

            return (highestOutputIndex, outputValue);
        }


        // Unit Movement
        private void UnitMove(UnitWrapper unit)
        {
            var highestOutput = HighestOutputGet(unit.Unit.BrainGraph.DecisionBrain);
            UnitOutputInterpret(unit, highestOutput.index);

            // Store positions to save in DB
            unit.XPos.Add(unit.CurrentPosition.GetCoordinate(0));
            unit.YPos.Add(unit.CurrentPosition.GetCoordinate(1));
        }
        private void Move(UnitWrapper unit, int dimensionIndex, float value)
        {
            var isMoveLegit = IsMoveLegit(unit, dimensionIndex, value);
            if (isMoveLegit.legit)
                unit.CurrentPosition.SetCoordinate(dimensionIndex, isMoveLegit.finalPosition);
            else
                Move(unit, dimensionIndex, -value);

        }
        private void RandomMove(UnitWrapper unit)
        {
            var availablePositions = new List<string> { "NoMove" };
            if (IsMoveLegit(unit, 0, 1).legit)
                availablePositions.Add("Right");
            if (IsMoveLegit(unit, 0, -1).legit)
                availablePositions.Add("Left");
            if (IsMoveLegit(unit, 1, 1).legit)
                availablePositions.Add("Up");
            if (IsMoveLegit(unit, 1, -1).legit)
                availablePositions.Add("Down");

            var index = new Random().Next(availablePositions.Count);
            switch(availablePositions[index])
            {
                case "Down":
                    Move(unit, 1, -1);
                    break;
                case "Up":
                    Move(unit, 1, -1);
                    break;
                case "Right":
                    Move(unit, 1, -1);
                    break;
                case "Left":
                    Move(unit, 1, -1);
                    break;
                case "NoMove":
                    break;
            }
        }
        private (bool legit, float finalPosition) IsMoveLegit(UnitWrapper unit, int dimensionIndex, float value)
        {
            var result = unit.CurrentPosition.GetCoordinate(dimensionIndex) + value;
            return (result <= _simulationParameters.SpaceDimensions[dimensionIndex].max && result >= _simulationParameters.SpaceDimensions[dimensionIndex].min, result);
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
                Units[i] = UnitBuild(childrenUnits[i]);
            for (int i = 0; i < randomChildren.Count; i++)
                Units[i + childrenUnits.Length] = randomChildren[i];

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
                    var mixedGenome = _genomeManager.GenomeCrossOverGet(genomeA, genomeB, _brainCaracteristic, 1, 0.001f);

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
        private List<UnitWrapper> SelectBestUnits(float percentageToSelect, bool sortScoreDesc = false)
        {
            var orderedUnits = sortScoreDesc ? Units.OrderByDescending(t => t.Score) : Units.OrderBy(t => t.Score);
            var survivorNumber = SurvivorNumberCount();
            var maxToTake = Math.Max(survivorNumber, percentageToSelect * _simulationParameters.PopulationNumber);
            var selected = orderedUnits.Take((int)maxToTake).ToList();

            return selected;
        }

        private float CircularScoreAssign()
        {
            var meanScore = 0f;
            var xCenter = _simulationParameters.xCenter;
            var yCenter = _simulationParameters.yCenter;
            for (int i = 0; i < _simulationParameters.PopulationNumber; i++)
            {
                var currentXpos = Units[i].CurrentPosition.GetCoordinate(0);
                var currentYpos = Units[i].CurrentPosition.GetCoordinate(1);

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
                var currentXpos = Units[i].CurrentPosition.GetCoordinate(0);
                var currentYpos = Units[i].CurrentPosition.GetCoordinate(1);

                var unitScore = (float)(Math.Pow(Math.Abs((currentXpos - _simulationParameters.RecXmin) - (_simulationParameters.RecXmax - currentXpos)), 2) + Math.Pow(Math.Abs((currentYpos - _simulationParameters.RecYmin) - (_simulationParameters.RecYmax - currentYpos)), 2));
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
    }
}
