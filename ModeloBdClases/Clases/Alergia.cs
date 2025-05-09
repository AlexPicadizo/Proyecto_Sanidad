using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloBdClases.Clases
{
    public class Alergia
    {
        // Atributos privados
        private int id;
        private string nombre;

        #region RELACIONES
        /// <summary>
        /// Lista de pacientes que tienen esta alergia.
        /// </summary>
        public List<PacienteAlergia> PacientesAlergia { get; set; }
        #endregion

        #region CONSTRUCTORES

        /// <summary>
        /// Constructor para crear una alergia sin ID (nuevo registro).
        /// </summary>
        /// <param name="nombre">Nombre de la alergia</param>
        public Alergia(string nombre)
        {
            Nombre = nombre;
        }

        /// <summary>
        /// Constructor para crear una alergia con ID (usado cuando se lee desde una base de datos).
        /// </summary>
        /// <param name="id">ID de la alergia</param>
        /// <param name="nombre">Nombre de la alergia</param>
        public Alergia(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        #endregion

        #region PROPIEDADES

        /// <summary>
        /// Identificador único de la alergia.
        /// </summary>
        public int Id
        {
            get => id;
            set => id = value;
        }

        /// <summary>
        /// Nombre de la alergia. Debe ser válido y no vacío.
        /// </summary>
        public string Nombre
        {
            get => ComprobarNombre(nombre);
            set => nombre = value;
        }

        #endregion

        #region METODOS

        /// <summary>
        /// Verifica que el nombre de la alergia no esté vacío ni nulo.
        /// </summary>
        /// <param name="nombre">El nombre de la alergia</param>
        /// <returns>El nombre de la alergia si es válido</returns>
        /// <exception cref="Exception">Si el nombre está vacío o nulo</exception>
        private string ComprobarNombre(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
                throw new Exception("El nombre de la alergia no puede estar vacío.");
            return nombre;
        }

        #endregion
    }
}
