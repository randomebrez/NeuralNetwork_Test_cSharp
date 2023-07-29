using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeuralNetwork_Test_cSharp.DTO.DatabaseModel
{
    [Table("generation_results")]
    public class GenerationResultDb
    {
        [Column("id"), Key, Required]
        public int Id { get; set; }

        [Column("generation_number"), Required]
        public int GenerationNumber { get; set; }

        [Column("simulation_id"), Required]
        public int SimulationId { get; set; }

        [Column("mean_score"), Required]
        public float MeanScore { get; set; }

        [Column("survivor_number"), Required]
        public int SurvivorNumber { get; set; }
    }
}
