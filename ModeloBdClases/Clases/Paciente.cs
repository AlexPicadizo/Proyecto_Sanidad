using System.Text.RegularExpressions;
using System.ComponentModel;
using ModeloBdClases.Validaciones;

namespace ModeloBdClases.Clases
{
    public class Paciente : INotifyPropertyChanged
    {
        #region EVENTOS
        /// <summary>
        /// Evento necesario para notificar cambios en propiedades, útil en interfaces con binding (por ejemplo, WPF).
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        // Atributos privados
        private string nombre, apellidos, grupoSanguineo, dni;
        private int edad;
        private DateTime fechaIngreso;
        private Ubicacion _ubicacion;

        #region RELACIONES
        /// <summary>
        /// Lista de alergias que tiene el paciente (relación muchos a muchos).
        /// </summary>
        public List<PacienteAlergia> AlergiasPaciente { get; set; }

        /// <summary>
        /// Tratamientos asignados al paciente.
        /// </summary>
        public List<Tratamiento> Tratamiento { get; set; }

        /// <summary>
        /// Diagnósticos realizados al paciente.
        /// </summary>
        public List<Diagnostico> Diagnosticos { get; set; }

        /// <summary>
        /// Notas de evolución médica del paciente.
        /// </summary>
        public List<NotaEvolucion> NotaEvolucion { get; set; }

        #endregion

        #region CONSTRUCTORES
        public Paciente()
        {
            // Constructor por defecto necesario para serialización, bindings y pruebas
        }


        /// <summary>
        /// Constructor principal que inicializa un paciente con los datos necesarios.
        /// </summary>
        /// <param name="nombre">Nombre del paciente.</param>
        /// <param name="apellidos">Apellidos del paciente.</param>
        /// <param name="edad">Edad del paciente.</param>
        /// <param name="dni">Documento nacional de identidad.</param>
        /// <param name="grupoSanguineo">Grupo sanguíneo del paciente.</param>
        /// <param name="fechaIngreso">Fecha de ingreso al centro médico.</param>
        public Paciente(string nombre, string apellidos, int edad, string dni, string grupoSanguineo, DateTime fechaIngreso)
        {
            Nombre = nombre;
            Apellidos = apellidos;
            Edad = edad;
            Dni = dni;
            GrupoSanguineo = grupoSanguineo;
            FechaIngreso = fechaIngreso;
        }
        #endregion

        #region PROPIEDADES
        public string Nombre
        {
            get => nombre;
            set => nombre = ValidadorDatos.ComprobarNombre(value);
        }

        public string Apellidos
        {
            get => apellidos;
            set => apellidos = ValidadorDatos.ComprobarApellidos(value);
        }

        public int Edad
        {
            get => edad;
            set => edad = ValidadorDatos.ComprobarEdad(value);
        }

        public string Dni
        {
            get => dni;
            set => dni = ValidadorDatos.ComprobarDni(value.Replace("-", ""));
        }

        public string GrupoSanguineo
        {
            get => grupoSanguineo;
            set => grupoSanguineo = ValidadorDatos.ComprobarGrupoSanguineo(value);
        }

        /// <summary>
        /// Ubicación física del paciente dentro del centro médico (por ejemplo, habitación).
        /// Incluye notificación para actualizaciones en interfaces (binding bidireccional).
        /// </summary>
        public Ubicacion Ubicacion
        {
            get => _ubicacion;
            set
            {
                // Verificamos si el nuevo valor de ubicación es diferente al actual
                if (_ubicacion != value)
                {
                    // Si ha cambiado, actualizamos el campo privado con el nuevo valor
                    _ubicacion = value;

                    // Llamamos al método OnPropertyChanged para notificar que la propiedad 'Ubicacion' ha cambiado.
                    // Esto permite que la interfaz de usuario se actualice automáticamente si está enlazada (binding)
                    OnPropertyChanged(nameof(Ubicacion));
                }

            }
        }


        public DateTime FechaIngreso
        {
            get => fechaIngreso;
            set => fechaIngreso = ValidadorDatos.ComprobarFechaIngreso(value);
        }

        #endregion

        #region INotifyPropertyChanged Helper
        /// <summary>
        /// Método que lanza el evento PropertyChanged para notificar a la UI que una propiedad ha cambiado.
        /// </summary>
        /// <param name="propiedad">Nombre de la propiedad que cambió.</param>
        protected void OnPropertyChanged(string propiedad)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        }
        #endregion

    }
}
