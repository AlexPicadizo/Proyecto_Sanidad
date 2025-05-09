using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloBdClases.Clases
{
    public class TratamientoMedicamento
    {
        #region PROPIEDADES
        /// <summary>
        /// Medicamento asociado al tratamiento.
        /// </summary>
        public Medicamento Medicamento { get; set; }

        /// <summary>
        /// Instrucciones específicas sobre cómo debe administrarse el medicamento.
        /// </summary>
        public string Instrucciones { get; set; }
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Constructor con los parámetros necesarios para crear una relación entre un 
        /// tratamiento y un medicamento.
        /// </summary>
        /// <param name="medicamento">Instancia del medicamento a administrar.</param>
        /// <param name="instrucciones">Instrucciones de administración del medicamento.</param>
        /// <exception cref="ArgumentNullException">Se lanza si el medicamento es nulo.</exception>
        /// <exception cref="Exception">Se lanza si las instrucciones están vacías.</exception>
        public TratamientoMedicamento(Medicamento medicamento, string instrucciones)
        {
            // Validar que el medicamento no sea nulo
            Medicamento = medicamento ?? throw new ArgumentNullException(nameof(medicamento));

            // Validar que las instrucciones no estén vacías
            Instrucciones = string.IsNullOrEmpty(instrucciones)
                ? throw new Exception("Las instrucciones no pueden estar vacías.")
                : instrucciones;
        }
        #endregion
    }
}
