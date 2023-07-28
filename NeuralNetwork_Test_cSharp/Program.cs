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

var connexionString = "D:\\Codes\\VisualStudio\\NeuralNetwork_Test_cSharp\\dataBase.db";
var simulationManager = new SimulationsManager(connexionString, false);

simulationManager.InitialyzeNewSimulationAsync(simulationParameters).GetAwaiter().GetResult();
simulationManager.ExecuteLifeAsync().GetAwaiter().GetResult();
