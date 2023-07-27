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
            await _databaseGateway.SimulationStoreAsync(simulationParameters).ConfigureAwait(false);
            //GetLastSimulationId
            _simulationParameters.SimulationId = await _databaseGateway.LastSimulationIdGetAsync().ConfigureAwait(false);

            //CreateFirstUnits
            _lifeManager = new LifeManager(simulationParameters);
            _lifeManager.InitialyzeUnits();
           
        }

        public async Task ExecuteLifeAsync()
        {
            var endCondition = false;
            while (endCondition == false)
            {
                var consoleLogs = new StringBuilder($"Starting generation {_lifeManager.GenerationId}\n");
                await _databaseGateway.UnitsStoreAsync(_lifeManager.Units).ConfigureAwait(false);
                
                
                _lifeManager.ExecuteGenerationLife();

                await _databaseGateway.UnitStepsStoreAsync(_lifeManager.Units);
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
