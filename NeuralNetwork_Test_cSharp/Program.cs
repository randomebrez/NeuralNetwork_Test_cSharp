using NeuralNetwork.Abstraction.Model;
using NeuralNetwork_Test_cSharp;
using NeuralNetwork_Test_cSharp.DTO;

var maxGeneration = 150;
var crossOverNumber = 1;
var mutationRate = 0.01f;
var genomeCaracteristics = new GenomeCaracteristics
{
    GeneNumber = 100,
    WeightMinimumValue = 0f,
    WeighBytesNumber = 4
};

var simulationParameters = new SimulationParameters
{
    PopulationNumber = 100,
    UnitLifeSpan = 100,
    SpaceDimensions = new Dictionary<int, (int min, int max)>
    {
        {0, (-50, 50)},
        {1, (-40, 40)},
    },
    //SelectionShape = SelectionShapeEnum.Rectangle,
    //RecXmin = -15,
    //RecXmax = 0,
    //RecYmin = 0,
    //RecYmax = 10
    SelectionShape = SelectionShapeEnum.Circular,
    Radius = 10,
    xCenter = 5,
    yCenter = -5
};
var brainCaracteristics = new BrainCaracteristics
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
                    NeuronNumber = 4,
                    ActivationFunction = ActivationFunctionEnum.Tanh,
                    ActivationFunction90PercentTreshold = 2f,
                }, },
    OutputLayer = new LayerCaracteristics(2, LayerTypeEnum.Output)
    {
        NeuronNumber = 7,
        ActivationFunction = ActivationFunctionEnum.Sigmoid,
        ActivationFunction90PercentTreshold = 1f,
        NeuronTreshold = 0f,
    },
    GenomeCaracteristics = genomeCaracteristics
};

var connexionString = "D:\\Codes\\VisualStudio\\NeuralNetwork_Test_cSharp\\dataBase.db";
var simulationManager = new SimulationsManager(connexionString, false);


simulationManager.InitialyzeNewSimulationAsync(simulationParameters, brainCaracteristics, crossOverNumber, mutationRate).GetAwaiter().GetResult();
simulationManager.ExecuteLifeAsync(maxGeneration).GetAwaiter().GetResult();
