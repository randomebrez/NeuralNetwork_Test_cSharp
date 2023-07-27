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
        private int _generationId;
        private List<string> _brainNames = new List<string> { "Main" };

        private BrainCaracteristics _caracteristic;

        private UnitWrapper[] _units;

        public LifeManager(SimulationParameters parameters)
        {
            _simulationParameters = parameters;
            
            _units = new UnitWrapper[parameters.PopulationNumber];
            _genomeManager = new GenomeManager();
            _brainCalculator = new BrainCalculator();
        }

        // Initialisation
        public List<UnitWrapper> InitialyzeUnits()
        {
            _generationId = 0;
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
                    NeuronNumber = 1,
                    ActivationFunction = ActivationFunctionEnum.Sigmoid,
                    ActivationFunction90PercentTreshold = 0.5f,
                }, },
                OutputLayer = new LayerCaracteristics(2, LayerTypeEnum.Output)
                {
                    NeuronNumber = 5,
                    ActivationFunction = ActivationFunctionEnum.Sigmoid,
                    ActivationFunction90PercentTreshold = 0.5f,
                    NeuronTreshold = 0f,
                },
                GenomeCaracteristics = new GenomeCaracteristics
                {
                    GeneNumber = 24,
                    WeighBytesNumber = 3
                }
            };

            var units = GenerateRandomUnits(_simulationParameters.PopulationNumber);

            for (int i = 0; i < _simulationParameters.PopulationNumber; i++)
                _units[i] = units[i];

            return _units.ToList();
        }

        private List<UnitWrapper> GenerateRandomUnits(int unitNumber)
        {
            var result = new List<UnitWrapper>();

            var genomes = _genomeManager.GenomesListGet(unitNumber, _caracteristic);
            var graphs = GenomeGraphGet(genomes);

            var units = _genomeManager.UnitsFromGenomeGraphList(graphs);

            for (int i = 0; i < unitNumber; i++)
                result.Add(new UnitWrapper
                {
                    Unit = units[i],
                    XPos = new List<float> { new Random().Next(_simulationParameters.Xmin, _simulationParameters.Xmax) },
                    YPos = new List<float> { new Random().Next(_simulationParameters.Ymin, _simulationParameters.Ymax) },
                    SimulationId = _simulationParameters.SimulationId,
                    GenerationId = _generationId
                }) ;

            return result;
        }

        private List<GenomeGraph> GenomeGraphGet(List<Genome> genomes)
        {
            var graphs = new List<GenomeGraph>();
            foreach (var genome in genomes)
                graphs.Add(new GenomeGraph
                {
                    GenomeNodes = new List<GenomeCaracteristicPair>
                    {
                        new GenomeCaracteristicPair
                        {
                            Caracteristics = _caracteristic,
                            Genome = genome
                        }
                    }
                });
            return graphs;
        }

        // Generation's Life Execution
        public void ExecuteGenerationLife()
        {
            var consoleLogs = new StringBuilder($"Starting generation {_generationId}");
            Console.WriteLine(consoleLogs.ToString());
            for (int i = 0; i < _simulationParameters.UnitLifeSpan; i++)
                ExecuteLifeStep();

            _generationId++;
        }

        private void ExecuteLifeStep()
        {
            for (int j = 0; j < _simulationParameters.PopulationNumber; j++)
            {
                var currentUnit = _units[j];
                var inputs = new Dictionary<string, List<float>>
                {
                    { "Main", GetInputs(currentUnit) }
                };

                _brainCalculator.BrainGraphCompute(currentUnit.Unit.BrainGraph, inputs);
                MoveUnit(currentUnit);
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

        private void MoveUnit(UnitWrapper unit)
        {
            var decisionBrain = unit.Unit.BrainGraph.DecisionBrain;
            var outputs = decisionBrain.Neurons.OutputLayer.Neurons.ToList();
            (int bestOutputIndex, float bestOutputValue) = (-1, 0);
            for (int i = 0; i < outputs.Count; i++)
            {
                if (outputs[i].Value > bestOutputValue)
                    (bestOutputIndex, bestOutputValue) = (outputs[i].Id, outputs[i].Value);
            }

            var currentXpos = unit.XPos[^1];
            var currentYpos = unit.YPos[^1];

            switch (bestOutputIndex)
            {
                case -1:
                    //_noMoveCount++;
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
                    unit.XPos.Add(currentXpos);
                    unit.YPos.Add(currentYpos);
                    break;
            }
        }

        // Reproduction
        public (int survivorNumber, int randomUnitNumber, float meanScore) ReproduceUnits()
        {
            //Select best units
            //var bestUnits = SelectBestUnitRightSide(48);
            //var bestUnits = SelectBestUnitSquare(40, 45, -10, -5);
            //var survivorNumber = bestUnits.Count;
            var (bestUnits, survivorNumber, meanScore) = SelectBestUnitCircle(_simulationParameters.Radius);

            var mixedGenomes = GetMixedGenomes(bestUnits);
            // Tres bizarre, il faut un graphe a suivre pour reconstruire à partir du dico de <brainName, List<Genome>>
            var genomes = mixedGenomes.ContainsKey("Main") ? mixedGenomes["Main"] : new List<Genome>();
            var graphs = GenomeGraphGet(genomes);
            var childrenUnits = _genomeManager.UnitsFromGenomeGraphList(graphs);
            var remainingUnitToGenerate = _simulationParameters.PopulationNumber - childrenUnits.Length;

            var randomChildren = GenerateRandomUnits(remainingUnitToGenerate);

            for(int i = 0; i < childrenUnits.Length; i++)
            {
                _units[i] = new UnitWrapper
                {
                    Unit = childrenUnits[i],
                    XPos = new List<float> { new Random().Next(_simulationParameters.Xmin, _simulationParameters.Xmax) },
                    YPos = new List<float> { new Random().Next(_simulationParameters.Ymin, _simulationParameters.Ymax) },
                    SimulationId = _simulationParameters.SimulationId,
                    GenerationId = _generationId
                };
            }
            for(int i = 0; i < randomChildren.Count; i++)
            {
                _units[i + childrenUnits.Length] = randomChildren[i];
            }

            return (survivorNumber, remainingUnitToGenerate, meanScore);
        }

        private Dictionary<string, List<Genome>> GetMixedGenomes(List<UnitWrapper> bestUnits)
        {
            var result = new Dictionary<string, List<Genome>>();
            var couples = CreateCouples(bestUnits);

            foreach (var couple in couples)
            {
                foreach (var brainName in _brainNames)
                {
                    var genomeA = couple.parentA.Unit.BrainGraph.BrainNodes[brainName].Genome;
                    var genomeB = couple.parentB.Unit.BrainGraph.BrainNodes[brainName].Genome;
                    var mixedGenome = _genomeManager.GenomeCrossOverGet(genomeA, genomeB, _caracteristic, 1, 0.01f);

                    if (result.TryGetValue(brainName, out var genomes))
                        genomes.Add(mixedGenome);
                    else
                        result.Add(brainName, new List<Genome> { mixedGenome });
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
        private List<UnitWrapper> SelectBestUnitRightSide(float xPosMin)
        {
            var survivors = new List<UnitWrapper>();
            for(int i = 0; i < _simulationParameters.PopulationNumber; i++)
            {
                if (_units[i].XPos[^1] > xPosMin)
                    survivors.Add(_units[i]);
            }

            return survivors;
        }

        private List<UnitWrapper> SelectBestUnitSquare(float xPosMin, float xPosMax, float yPosMin, float yPosMax)
        {
            var survivors = new List<UnitWrapper>();
            for (int i = 0; i < _simulationParameters.PopulationNumber; i++)
            {
                var currentXpos = _units[i].XPos[^1];
                var currentYpos = _units[i].YPos[^1];
                if (currentXpos >= xPosMin && currentXpos <= xPosMax && currentYpos >= yPosMin && currentYpos <= yPosMax)
                    survivors.Add(_units[i]);
            }

            return survivors;
        }

        private (List<UnitWrapper> selected, int survivors, float meanScore) SelectBestUnitCircle(float radius)
        {
            var selected = new List<UnitWrapper>();
            var survivorNumber = 0;
            var meanScore = 0f;
            for (int i = 0; i < _simulationParameters.PopulationNumber; i++)
            {
                var currentXpos = _units[i].XPos[^1];
                var currentYpos = _units[i].YPos[^1];

                var radiusPos = currentXpos * currentXpos + currentYpos * currentYpos;
                _units[i].Score = radiusPos;

                meanScore += radiusPos;
            }

            var orderedUnits = _units.OrderBy(t => t.Score).ToList();
            var selectedUnits = orderedUnits.Where(t => t.Score < radius * radius);
            survivorNumber = selectedUnits.Count();

            if (selectedUnits.Count() < 0.4f * _simulationParameters.PopulationNumber)
                selected = orderedUnits.Take((int)(0.4f * _simulationParameters.PopulationNumber)).ToList();
            else
                selected = selectedUnits.ToList();

            return (selected, survivorNumber, meanScore / _simulationParameters.PopulationNumber);
        }
    }
}
