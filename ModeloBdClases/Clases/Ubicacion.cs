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
            set => planta = ComprobarPlanta(value);
        }

        public int Habitacion
        {
            get => habitacion;
            set => habitacion = ComprobarHabitacion(value);
        }

        public int Cama
        {
            get => cama;
            set => cama = ComprobarCama(value);
        }
        #endregion

        #region VALIDACIONES
        /// <summary>
        /// Valida que la planta sea un valor positivo.
        /// </summary>
        private int ComprobarPlanta(int planta)
        {
            if (planta <= 0) throw new Exception("La planta debe ser un número mayor a 0.");
            if (planta > 26) throw new Exception("La planta debe ser un número menor a 26.");
            return planta;
        }

        /// <summary>
        /// Valida que la habitación sea un número positivo.
        /// </summary>
        private int ComprobarHabitacion(int habitacion)
        {
            if (habitacion <= 0) throw new Exception("La habitación debe ser un número mayor a 0.");
            return habitacion;
        }

        /// <summary>
        /// Valida que la cama sea un número positivo.
        /// </summary>
        private int ComprobarCama(int cama)
        {
            if (cama <= 0) throw new Exception("La cama debe ser un número mayor a 0.");
            if (cama > 6) throw new Exception("La cama debe ser un número menor a 6.");
            return cama;
        }
        #endregion

    }
}
