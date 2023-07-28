using NeuralNetwork.Abstraction.Model;
using NeuralNetwork_Test_cSharp.DTO;
using System.Text;

namespace NeuralNetwork_Test_cSharp
{
    public class SimulationsManager
    {
        private LifeManager _lifeManager;
        private DatabaseGateway _databaseGateway;

        private float _endConditionTreshold;

        public SimulationsManager(string connexionString, bool cleanDatabase)
        {
            // File is automatically recreated when instantiating the 'context'
            if (cleanDatabase && File.Exists(connexionString))
                File.Delete(connexionString);

            _databaseGateway = new DatabaseGateway(connexionString);
        }


        public async Task InitialyzeNewSimulationAsync(SimulationParameters simulationParameters, BrainCaracteristics brainCaracteristics, float endConditionTreshold = 0.95f)
        {
            _endConditionTreshold = endConditionTreshold;

            //Store simulation parameters
            await _databaseGateway.SimulationStoreAsync(simulationParameters).ConfigureAwait(false);
            //GetLastSimulationId
            simulationParameters.SimulationId = await _databaseGateway.LastSimulationIdGetAsync().ConfigureAwait(false);

            //CreateFirstUnits
            _lifeManager = new LifeManager(simulationParameters, brainCaracteristics);
            _lifeManager.InitialyzeUnits();
           
        }

        public async Task ExecuteLifeAsync()
        {
            var endCondition = false;
            while (endCondition == false)
            {
                var consoleLogs = new StringBuilder($"Starting generation {_lifeManager.GenerationId}\n");             
                
                _lifeManager.ExecuteGenerationLife();
                var meanScore = _lifeManager.UnitScoreAssign();

                await StoreGenerationDatasAsync().ConfigureAwait(false);

                var survivorNumber = _lifeManager.SurvivorNumberCount();
                var randomUnitNumber = _lifeManager.ReproduceUnits();
                endCondition = ((float)survivorNumber / _lifeManager.PopulationNumber) >= _endConditionTreshold;

                consoleLogs.AppendLine($"Survivor number : {survivorNumber}");
                consoleLogs.AppendLine($"Random unit number : {randomUnitNumber}");
                consoleLogs.AppendLine($"Mean score : {meanScore}");
                consoleLogs.AppendLine();
                Console.WriteLine(consoleLogs.ToString());
            }
            Console.WriteLine("gg");
            
        }

        
        // Called after each generation
        private async Task StoreGenerationDatasAsync()
        {
            await _databaseGateway.UnitsStoreAsync(_lifeManager.Units).ConfigureAwait(false);
            await _databaseGateway.UnitStepsStoreAsync(_lifeManager.Units);
        }
    }
}
