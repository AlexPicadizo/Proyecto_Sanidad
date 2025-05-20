using ModeloBdClases.Validaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloBdClases.Clases
{
    public class Medicamento
    {
        // Atributos privados
        private string nombre, descripcion;

        #region CONSTRUCTORES
        /// <summary>
        /// Constructor con los parámetros necesarios para la creación de un medicamento.
        /// </summary>
        /// <param name="nombre">Nombre del medicamento</param>
        /// <param name="descripcion">Descripción del medicamento</param>
        public Medicamento(string nombre, string descripcion)
        {
            Nombre = nombre;
            Descripcion = descripcion;
        }

        /// <summary>
        /// Constructor con identificador, utilizado por ejemplo al obtener datos desde la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del medicamento</param>
        /// <param name="nombre">Nombre del medicamento</param>
        /// <param name="descripcion">Descripción del medicamento</param>
        public Medicamento(int id, string nombre, string descripcion)
        {
            Id = id;
            Nombre = nombre;
            Descripcion = descripcion;
        }
        #endregion

        #region PROPIEDADES
        /// <summary>
        /// Identificador del medicamento (clave primaria).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del medicamento.
        /// No puede ser vacío.
        /// </summary>
        public string Nombre
        {
            get => nombre;
            set => nombre = ValidadorDatos.ComprobarNombreMedicamento(value); // Validación al asignar
        }

        /// <summary>
        /// Descripción general del medicamento.
        /// No puede estar vacía.
        /// </summary>
        public string Descripcion
        {
            get => descripcion;
            set => descripcion = ValidadorDatos.ComprobarDescripcionMedicamento(value); // Validación al asignar
        }
        #endregion


    }
}
