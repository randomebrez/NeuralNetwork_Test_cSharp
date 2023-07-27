using NeuralNetwork_Test_cSharp.DTO;
using System.Text;

namespace NeuralNetwork_Test_cSharp
{
    public class SimulationsManager
    {
        private LifeManager _lifeManager;
        private DatabaseGateway _databaseGateway;

        private SimulationParameters _simulationParameters;
        private float _endConditionTreshold = 0.95f;

        public SimulationsManager(string connexionString, bool cleanDatabase)
        {
            // File is automatically recreated when instantiating the 'context'
            if (cleanDatabase && File.Exists(connexionString))
                File.Delete(connexionString);

            _databaseGateway = new DatabaseGateway(connexionString);
        }

        public async Task InitialyzeNewSimulationAsync(SimulationParameters simulationParameters)
        {
            _simulationParameters = simulationParameters;
            //Store simulation parameters
            await _databaseGateway.SimulationSaveAsync(simulationParameters).ConfigureAwait(false);
            //GetLastSimulationId
            var simulationId = await _databaseGateway.LastSimulationIdGetAsync().ConfigureAwait(false);

            //CreateFirstUnits
            _lifeManager = new LifeManager(simulationParameters);
            var units = _lifeManager.InitialyzeUnits();
            //Store new units
            await _databaseGateway.UnitsSaveAsync(units).ConfigureAwait(false);
        }

        public void ExecuteLife()
        {
            var endCondition = false;
            while (endCondition == false)
            {
                var consoleLogs = new StringBuilder();
                _lifeManager.ExecuteGenerationLife();
                
                var (survivorNumber, randomUnitNumber, meanScore) = _lifeManager.ReproduceUnits();
                endCondition = ((float)survivorNumber / _simulationParameters.PopulationNumber) >= _endConditionTreshold;

                consoleLogs.AppendLine($"Survivor number : {survivorNumber}");
                consoleLogs.AppendLine($"Random unit number : {randomUnitNumber}");
                consoleLogs.AppendLine($"Mean score : {meanScore}");
                consoleLogs.AppendLine();
                Console.WriteLine(consoleLogs.ToString());
            }
            Console.WriteLine("gg");
            
        }

        

        // Called after each generation
        public void StoreDatas()
        {
            // Store unit steps
        }
    }
}
