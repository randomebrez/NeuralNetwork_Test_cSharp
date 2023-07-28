using Microsoft.EntityFrameworkCore;
using NeuralNetwork_Test_cSharp.DTO;
using NeuralNetwork_Test_cSharp.DTO.DatabaseModel;

namespace NeuralNetwork_Test_cSharp
{
    public class DatabaseGateway
    {
        private readonly Context _context;

        public DatabaseGateway(string connexionString)
        {
            _context = new Context(connexionString);
            _context.Database.EnsureCreated();
        }   


        public async Task SimulationStoreAsync(SimulationParameters simulationParameters)
        {
            try
            {
                await _context.Simulations.AddAsync(DbMapper.ToDb(simulationParameters)).ConfigureAwait(false);                
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch(Exception e)
            {
                throw new Exception("Error while adding a new simulation entry in database", e);
            }
        }
        public async Task<int> LastSimulationIdGetAsync()
        {
            try
            {
                var simulationIds = (await _context.Simulations.ToListAsync().ConfigureAwait(false)).Select(t => t.Id);
                return simulationIds.OrderByDescending(t => t).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new Exception("Error while fetching last simulation ID", e);
            }
        }
        public async Task UnitsStoreAsync(UnitWrapper[] units)
        {
            try
            {
                var dbUnits = units.Select(t => DbMapper.ToDb(t));
                await _context.Units.AddRangeAsync(dbUnits).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                ClearChangeTracker();
            }
            catch (Exception e)
            {
                throw new Exception("Error while adding new unit entries in database", e);
            }
        }
        public async Task UnitStepsStoreAsync(UnitWrapper[] units)
        {
            try
            {
                var dbUnitSteps = new List<UnitStepDb>();
                for(int i = 0; i < units.Length; i++)
                    dbUnitSteps.AddRange(DbMapper.ToDb(units[i].Identifier.ToString(), units[i].XPos, units[i].YPos));

                await _context.UnitSteps.AddRangeAsync(dbUnitSteps).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                ClearChangeTracker();
            }
            catch (Exception e)
            {
                throw new Exception("Error while adding unit step entries in database", e);
            }
        }

        private void ClearChangeTracker()
        {
            _context.ChangeTracker.Clear();
        }
    }
}
