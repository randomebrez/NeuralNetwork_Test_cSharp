using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NeuralNetwork_Test_cSharp.DTO.DatabaseModel
{
    [Table("units")]
    public class UnitDb
    {
        [Column("unit_id"), Key, Required]
        public int Id { get; set; }

        [Column("unit_identifier"), Required]
        public string Identifier { get; set; }

        [Column("generation_id"), Required]
        public int GenerationId { get; set; }

        [Column("selection_score")]
        public float? SelectionScore { get; set; }

        [Column("simulation_id"), Required]
        public int SimulationId { get; set; }
    }
}
