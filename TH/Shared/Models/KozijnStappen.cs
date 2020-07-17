using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TH.Shared.Models
{
    [Table("kozijn_stappen")]
    public class KozijnStappen
    {
        [Key, Column("Id")]
        /// <summary> THe unique identifier of the kozijnStap. </summary>
        public int Id { get; set; }

        [Column("Type")]
        /// <summary> The type of step. </summary>
        public int StepType { get; set; }

        [Column("Step")]
        /// <summary> The Step index. </summary>
        public int StepCount { get; set; }

        [Column("Description")]
        /// <summary> The description of the step. </summary>
        public string Description { get; set; }

        /// <summary> The possible types of steps. </summary>
        public enum Ref
        {
            Raamkozijn = 1,
            Schuifpui = 2
        }
    }
}
