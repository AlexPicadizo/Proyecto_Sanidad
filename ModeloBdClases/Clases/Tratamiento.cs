using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloBdClases.Clases
{
    public class Tratamiento
    {
        // Atributos privados
        private DateTime? fechaInicio, fechaFin;

        #region RELACIONES
        /// <summary>
        /// Diagnóstico al que está asociado este tratamiento.
        /// Un tratamiento puede derivarse de un diagnóstico específico.
        /// </summary>
        public Diagnostico Diagnostico { get; set; }

        /// <summary>
        /// Lista de medicamentos asociados al tratamiento junto con sus instrucciones.
        /// </summary>
        public List<TratamientoMedicamento> Medicamentos { get; set; }
        #endregion

        #region CONSTRUCTORES
        /// <summary>
        /// Constructor para crear un tratamiento sin especificar ID, por ejemplo, antes de guardarlo en base de datos.
        /// </summary>
        /// <param name="nombre">Nombre del tratamiento.</param>
        /// <param name="fechaInicio">Fecha en la que inicia el tratamiento.</param>
        /// <param name="fechaFin">Fecha en la que finaliza el tratamiento.</param>
        public Tratamiento(string nombre, DateTime? fechaInicio, DateTime? fechaFin)
        {
            Nombre = nombre;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
        }

        /// <summary>
        /// Constructor con ID, útil cuando se obtiene el tratamiento desde la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del tratamiento.</param>
        /// <param name="nombre">Nombre del tratamiento.</param>
        /// <param name="fechaInicio">Fecha de inicio del tratamiento.</param>
        /// <param name="fechaFin">Fecha de finalización del tratamiento.</param>
        public Tratamiento(int id, string nombre, DateTime? fechaInicio, DateTime? fechaFin)
        {
            Id = id;
            Nombre = nombre;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
        }
        #endregion

        #region PROPIEDADES
        /// <summary>
        /// Nombre del tratamiento.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Fecha en la que el tratamiento comienza.
        /// </summary>
        public DateTime? FechaInicio
        {
            get => fechaInicio;
            set => fechaInicio = value;
        }

        /// <summary>
        /// Fecha en la que el tratamiento finaliza.
        /// </summary>
        public DateTime? FechaFin
        {
            get => fechaFin;
            set => fechaFin = value;
        }

        /// <summary>
        /// Identificador único del tratamiento (por ejemplo, ID en base de datos).
        /// </summary>
        public int Id { get; set; }
        #endregion

        #region VALIDACIONES

        /// <summary>
        /// Valida que la fecha de inicio no sea posterior a la fecha de fin.
        /// </summary>
        /// <exception cref="Exception">Se lanza si la fecha de inicio es después de la fecha de fin.</exception>
        public void ValidarFechas()
        {
            if (FechaInicio.HasValue && FechaFin.HasValue && FechaInicio > FechaFin)
            {
                throw new Exception("La fecha de inicio no puede ser posterior a la fecha de fin.");
            }
        }

        #endregion
    }
}
