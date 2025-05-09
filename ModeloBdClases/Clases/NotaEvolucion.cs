using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloBdClases.Clases
{
    public class NotaEvolucion
    {
        // Atributos privados
        private DateTime fecha;
        private string contenido;

        #region RELACIONES
        // Relación con el paciente al que pertenece esta nota
        public Paciente Paciente { get; set; }
        #endregion

        #region CONSTRUCTOR
        public NotaEvolucion(DateTime fecha, string contenido)
        {
            Fecha = fecha;
            Contenido = contenido;
        }

        public NotaEvolucion(int id, DateTime fecha, string contenido)
        {
            Id = id;
            Fecha = fecha;
            Contenido = contenido;
        }
        #endregion

        #region PROPIEDADES
        public int Id { get; set; }

        public DateTime Fecha
        {
            get => fecha;
            set => fecha = ValidarFecha(value);
        }

        public string Contenido
        {
            get => contenido;
            set => contenido = ComprobarContenido(value);
        }
        #endregion

        #region VALIDACIONES
        private string ComprobarContenido(string contenido)
        {
            if (string.IsNullOrEmpty(contenido))
                throw new Exception("El contenido de la nota no puede estar vacío.");
            return contenido;
        }

        private DateTime ValidarFecha(DateTime fecha)
        {
            if (fecha > DateTime.Now)
                throw new Exception("La fecha de la nota no puede ser futura.");
            return fecha;
        }
        #endregion
    }
}
