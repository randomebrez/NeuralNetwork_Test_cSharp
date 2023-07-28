using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NeuralNetwork_Test_cSharp.DTO.DatabaseModel
{
    [Table("simulations")]
    public class SimulationDb
    {
        [Column("simulation_id"), Key, Required]
        public int Id { get; set; }

        [Column("environment_limits"), Required]
        public string EnvironmentLimits { get; set; }

        [Column("selection_shape"), Required]
        public string SelectionShape { get; set; }

        [Column("selection_constraints"), Required]
        public string SelectionConstraints { get; set; }

        [Column("unit_life_span"), Required]
        public int UnitLifeSpan { get; set; }

        //[ForeignKey(nameof(UnitId))]
        //public UnitDb Unit { get; set; }
    }
}
