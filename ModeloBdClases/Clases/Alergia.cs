using ModeloBdClases.Validaciones;
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
            get => nombre;
            set => nombre = value; // ValidadorDatos.ComprobarNombreAlergia(nombre);
        }

        #endregion


    }
}
