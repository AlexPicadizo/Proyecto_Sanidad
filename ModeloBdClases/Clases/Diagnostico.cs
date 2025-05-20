using ModeloBdClases.Validaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloBdClases.Clases
{
    public class Diagnostico
    {
        // Atributos privados
        private string nombre, descripcion;
        private DateTime fechaDiagnostico;

        #region RELACIONES
        /// <summary>
        /// Paciente al que se le ha realizado este diagnóstico.
        /// </summary>
        public Paciente Paciente { get; set; }

        /// <summary>
        /// Lista de tratamientos asociados a este diagnóstico.
        /// </summary>
        public List<Tratamiento> Tratamientos { get; set; }
        #endregion

        #region CONSTRUCTORES

        /// <summary>
        /// Constructor para crear un nuevo diagnóstico (sin ID).
        /// </summary>
        /// <param name="nombre">Nombre del diagnóstico</param>
        /// <param name="descripcion">Descripción del diagnóstico</param>
        /// <param name="fecha">Fecha del diagnóstico</param>
        public Diagnostico(string nombre, string descripcion, DateTime fecha)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            FechaDiagnostico = fecha;
            Tratamientos = new List<Tratamiento>();
        }

        /// <summary>
        /// Constructor completo, utilizado cuando ya se conoce el ID (por ejemplo, desde base de datos).
        /// </summary>
        /// <param name="id">ID único del diagnóstico</param>
        /// <param name="nombre">Nombre del diagnóstico</param>
        /// <param name="descripcion">Descripción del diagnóstico</param>
        /// <param name="fecha">Fecha del diagnóstico</param>
        public Diagnostico(int id, string nombre, string descripcion, DateTime fecha)
        {
            Id = id;
            Nombre = nombre;
            Descripcion = descripcion;
            FechaDiagnostico = fecha;
            Tratamientos = new List<Tratamiento>();
        }

        #endregion

        #region PROPIEDADES

        /// <summary>
        /// Identificador único del diagnóstico.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del diagnóstico. No puede estar vacío.
        /// </summary>
        public string Nombre
        {
            get => nombre;
            set => nombre = ValidadorDatos.ComprobarNombreDiagnostico(value);
        }

        /// <summary>
        /// Descripción detallada del diagnóstico. No puede estar vacía.
        /// </summary>
        public string Descripcion
        {
            get => descripcion;
            set => descripcion = ValidadorDatos.ComprobarDescripcionDiagnostico(value);
        }

        /// <summary>
        /// Fecha en la que se realizó el diagnóstico. No puede ser futura.
        /// </summary>
        public DateTime FechaDiagnostico
        {
            get => fechaDiagnostico;
            set => fechaDiagnostico = ValidadorDatos.ComprobarFechaDiagnostico(value);
        }

        #endregion

    }
}

