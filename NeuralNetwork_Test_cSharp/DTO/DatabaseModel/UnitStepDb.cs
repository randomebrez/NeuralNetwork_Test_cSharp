using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeuralNetwork_Test_cSharp.DTO.DatabaseModel
{
    [Table("unit_steps")]
    public class UnitStepDb
    {
        [Column("step_id"), Key, Required]
        public int Id { get; set; }

        [Column("unit_identifier"), Required]
        public string UnitIdentifier { get; set; }

        [Column("life_step_id"), Required]
        public int LifeStepId { get; set; }

        [Column("position"), Required]
        public string Position { get; set; }
    }
}
