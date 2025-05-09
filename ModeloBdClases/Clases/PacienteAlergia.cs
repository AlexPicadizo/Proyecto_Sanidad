using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloBdClases.Clases
{
    public class PacienteAlergia
    {
        #region PROPIEDADES
        /// <summary>
        /// Identificador del paciente que tiene la alergia.
        /// </summary>
        public int PacienteId { get; set; }

        /// <summary>
        /// Referencia al objeto Paciente.
        /// Permite acceder a los datos del paciente que tiene esta alergia.
        /// </summary>
        public Paciente Paciente { get; set; }

        /// <summary>
        /// Identificador de la alergia asociada al paciente.
        /// </summary>
        public int AlergiaId { get; set; }

        /// <summary>
        /// Referencia al objeto Alergia.
        /// Permite acceder a la información detallada de la alergia.
        /// </summary>
        public Alergia Alergia { get; set; }
        #endregion
    }

}
