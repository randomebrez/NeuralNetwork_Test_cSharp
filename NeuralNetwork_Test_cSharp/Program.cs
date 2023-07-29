using NeuralNetwork.Abstraction.Model;
using NeuralNetwork_Test_cSharp;
using NeuralNetwork_Test_cSharp.DTO;

var simulationParameters = new SimulationParameters
{
    PopulationNumber = 150,
    UnitLifeSpan = 80,
    SpaceDimensions = new Dictionary<int, (int min, int max)>
    {
        {0, (-50, 50)},
        {1, (-40, 40)},
    },
    SelectionShape = SelectionShapeEnum.Rectangle,
    RecXmin = -15,
    RecXmax = 0,
    RecYmin = 0,
    RecYmax = 10
    //SelectionShape = SelectionShapeEnum.Circular,
    //Radius = 10,
    //xCenter = 0,
    //yCenter = 0
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
                    NeuronNumber = 1,
                    ActivationFunction = ActivationFunctionEnum.Tanh,
                    ActivationFunction90PercentTreshold = 2f,
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
        GeneNumber = 100,
        WeighBytesNumber = 3
    }
};

var connexionString = "D:\\Codes\\VisualStudio\\NeuralNetwork_Test_cSharp\\dataBase.db";
var simulationManager = new SimulationsManager(connexionString, false);

simulationManager.InitialyzeNewSimulationAsync(simulationParameters, brainCaracteristics).GetAwaiter().GetResult();
simulationManager.ExecuteLifeAsync().GetAwaiter().GetResult();
