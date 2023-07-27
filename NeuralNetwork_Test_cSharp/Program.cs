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

var connexionString = "C:\\Users\\nico-\\Documents\\Codes\\Visual Studio 2022\\Projects\\NeuralNetwork_Test_cSharp\\dataBase.db";
var simulationManager = new SimulationsManager(connexionString, false);

simulationManager.InitialyzeNewSimulationAsync(simulationParameters).GetAwaiter().GetResult();
simulationManager.ExecuteLife();
