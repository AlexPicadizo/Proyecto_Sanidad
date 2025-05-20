using ModeloBdClases.Validaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloBdClases.Clases
{
    public class Ubicacion
    {
        private int planta, habitacion, cama;

        #region RELACIONES
        // Relación inversa: un paciente tiene una única ubicación
        public Paciente Paciente { get; set; }
        #endregion

        #region CONSTRUCTORES
        /// <summary>
        /// Constructor con los parámetros necesarios para la creación de una ubicación.
        /// </summary>
        public Ubicacion(int planta, int habitacion, int cama)
        {
            Planta = planta;
            Habitacion = habitacion;
            Cama = cama;
        }
        #endregion

        #region PROPIEDADES
        public int Planta
        {
            get => planta;
            set => planta = ValidadorDatos.ComprobarPlanta(value);
        }

        public int Habitacion
        {
            get => habitacion;
            set => habitacion = ValidadorDatos.ComprobarHabitacion(value);
        }

        public int Cama
        {
            get => cama;
            set => cama = ValidadorDatos.ComprobarCama(value);
        }
        #endregion

        #region VALIDACIONES

        #endregion

    }
}
