﻿using Microsoft.EntityFrameworkCore;
using NeuralNetwork_Test_cSharp.DTO;

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

        public async Task SimulationSaveAsync(SimulationParameters simulationParameters)
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
                return simulationIds.OrderByDescending(t => t).First();
                
            }
            catch (Exception e)
            {
                throw new Exception("Error while fetching last simulation ID", e);
            }
        }

        public async Task UnitsSaveAsync(List<UnitWrapper> units)
        {
            try
            {
                var dbUnits = units.Select(t => DbMapper.ToDb(t));
                await _context.Units.AddRangeAsync(dbUnits).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new Exception("Error while adding new unit entries in database", e);
            }
        }

        public async Task UnitStepsSaveAsync(UnitWrapper unit)
        {
            try
            {
                var dbUnitSteps = DbMapper.ToDb(unit.Identifier.ToString(), unit.XPos, unit.YPos);
                for(int i = 0; i < dbUnitSteps.Count(); i++)
                    await _context.UnitSteps.AddRangeAsync(dbUnitSteps[i]).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw new Exception("Error while adding unit step entries in database", e);
            }
        }
    }
}
