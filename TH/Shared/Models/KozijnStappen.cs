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
        /// <summary> THe unique identifier of the kozijnStap. </summary>
        [Key, Column("Id")]
        public int Id { get; set; }

        /// <summary> The type of step. </summary>
        [Column("Type")]
        public int StepType { get; set; }

        /// <summary> The Step index. </summary>
        [Column("Step")]
        public int StepCount { get; set; }

        /// <summary> The description of the step. </summary>
        [Column("Description")]
        public string Description { get; set; }

        /// <summary> The possible types of steps. </summary>
        public enum Ref
        {
            Raamkozijn = 1,
            Schuifpui = 2
        }
    }
}
