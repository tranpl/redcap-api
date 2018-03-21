using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Redcap.Models
{
    /// <summary>
    /// raw [default], label - export the raw coded values or labels for the options of multiple choice fields
    /// </summary>
    public enum ProjectPurpose
    {
        /// <summary>
        /// Pratice 
        /// </summary>
        [Description("Pratice or For Fun")]
        Pratice_ForFun = 0,

        /// <summary>
        /// Other
        /// </summary>
        [Description("Other")]
        Other = 1,

        /// <summary>
        /// Research
        /// </summary>
        [Description("Research")]
        Research = 2,

        /// <summary>
        /// Quality Improvement
        /// </summary>
        [Description("Quality Improvement")]
        QualityImprovement = 3,

        /// <summary>
        /// Other
        /// </summary>
        [Description("Operational Support")]
        OperationalSupport = 4
    }
}
