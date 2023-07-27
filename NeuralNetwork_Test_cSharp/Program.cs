using NeuralNetwork_Test_cSharp;
using NeuralNetwork_Test_cSharp.DTO;

var simulationParameters = new SimulationParameters
{
    PopulationNumber = 100,
    UnitLifeSpan = 100,
    Xmin = -50,
    Xmax = 50,
    Ymin = -50,
    Ymax = 50,
    SelectionShape = SelectionShapeEnum.Circular,
    Radius = 10,
    Center = 0,
};

var connexionString = "D:\\Codes\\VisualStudio\\NeuralNetwork_Test_cSharp\\dataBase.db";
var simulationManager = new SimulationsManager(connexionString, false);

simulationManager.InitialyzeNewSimulationAsync(simulationParameters).GetAwaiter().GetResult();
simulationManager.ExecuteLifeAsync().GetAwaiter().GetResult();
