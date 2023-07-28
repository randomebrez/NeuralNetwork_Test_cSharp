using NeuralNetwork.Abstraction.Model;
using NeuralNetwork_Test_cSharp;
using NeuralNetwork_Test_cSharp.DTO;

var simulationParameters = new SimulationParameters
{
    PopulationNumber = 200,
    UnitLifeSpan = 100,
    Xmin = -50,
    Xmax = 50,
    Ymin = -50,
    Ymax = 50,
    SelectionShape = SelectionShapeEnum.Rectangle,
    RecXmin = -15,
    RecXmax = 0,
    RecYmin = 0,
    RecYmax = 10
    //SelectionShape = SelectionShapeEnum.Circular,
    //Radius = 10,
    //xCenter = 40,
    //yCenter = -40
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

var connexionString = "D:\\Codes\\VisualStudio\\NeuralNetwork_Test_cSharp\\dataBase.db";
var simulationManager = new SimulationsManager(connexionString, false);

simulationManager.InitialyzeNewSimulationAsync(simulationParameters, brainCaracteristics).GetAwaiter().GetResult();
simulationManager.ExecuteLifeAsync().GetAwaiter().GetResult();
