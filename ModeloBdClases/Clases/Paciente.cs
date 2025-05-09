using System.Text.RegularExpressions;
using System.ComponentModel;

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
            set => nombre = ComprobarNombre(value);
        }

        public string Apellidos
        {
            get => apellidos;
            set => apellidos = ComprobarApellidos(value);
        }

        public int Edad
        {
            get => edad;
            set => edad = ComprobarEdad(value);
        }

        public string Dni
        {
            get => dni;
            set => dni = ComprobarDni(value);
        }

        public string GrupoSanguineo
        {
            get => grupoSanguineo;
            set => grupoSanguineo = ComprobarGrupoSanguineo(value);
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
            set => fechaIngreso = ComprobarFechaIngreso(value);
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

        #region VALIDACIONES
        /// <summary>
        /// Valida que el nombre contenga solo letras, tildes y espacios.
        /// </summary>
        private string ComprobarNombre(string Nombre)
        {
            if (string.IsNullOrEmpty(Nombre)) throw new Exception("El nombre no puede estar vacío.");
            Regex regex = new Regex(@"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ ]+$");
            if (!regex.IsMatch(Nombre)) throw new Exception("El nombre no puede contener números ni caracteres especiales.");
            return Nombre;
        }

        /// <summary>
        /// Valida que los apellidos contengan solo letras, tildes y espacios.
        /// </summary>
        private string ComprobarApellidos(string Apellido)
        {
            if (string.IsNullOrEmpty(Apellido)) throw new Exception("Los apellidos no pueden estar vacíos.");
            Regex regex = new Regex(@"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ ]+$");
            if (!regex.IsMatch(Apellido)) throw new Exception("Los apellidos no pueden contener números ni caracteres especiales.");
            return Apellido;
        }

        /// <summary>
        /// Valida que la edad sea un valor mayor a cero.
        /// </summary>
        private int ComprobarEdad(int edad)
        {
            if (edad <= 0) throw new Exception("La edad debe ser mayor a 0.");

            // Validar la edad antes de convertir (evitar FormatException)
            if (!int.TryParse(edad.ToString(), out edad) || edad <= 0)
            {
                throw new Exception("La edad debe ser un número mayor a 0.");
            }

            return edad;
        }

        /// <summary>
        /// Comprueba que el DNI tenga el formato correcto
        /// </summary>
        /// <param name="dni"> El DNI</param>
        /// <returns> El DNI</returns>
        /// <exception cref="Exception"> Si no es correcto lo notifica </exception>
        private string ComprobarDni(string dni)
        {
            // 1) Comprobar que no esté vacío
            if (string.IsNullOrEmpty(dni))
                throw new Exception("El DNI no puede estar vacío.");

            // 2) Validar formato básico: 8 dígitos seguidos de una letra
            if (!Regex.IsMatch(dni, @"^\d{8}[A-Za-z]$"))
                throw new Exception("El DNI debe tener 8 números seguidos de una letra (Ejemplo: 12345678A).");

            // 3) Separar la parte numérica y la letra
            string numeroStr = dni.Substring(0, 8);
            char letra = char.ToUpper(dni[8]);

            // 4) Calcular índice usando el resto de la división entre 23
            int numero = int.Parse(numeroStr);
            int indice = numero % 23;

            // 5) Tabla de letras correspondiente al DNI español
            const string tablaLetras = "TRWAGMYFPDXBNJZSQVHLCKE";
            char letraCorrecta = tablaLetras[indice];

            // 6) Comparar la letra calculada con la proporcionada
            if (letra != letraCorrecta)
                throw new Exception($"Letra incorrecta del DNI. La letra correcta para {numeroStr} es '{letraCorrecta}'.");

            // 7) Devolver en mayúsculas
            return numeroStr + letraCorrecta;
        }

        /// <summary>
        /// Valida que el grupo sanguíneo sea uno de los valores aceptados.
        /// </summary>
        private string ComprobarGrupoSanguineo(string grupo)
        {

            if (string.IsNullOrEmpty(grupo))
                throw new Exception("Debe seleccionar un grupo sanguíneo.");

            string[] gruposValidos = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            if (Array.IndexOf(gruposValidos, grupo.ToUpper()) == -1)
                throw new Exception("El grupo sanguíneo debe ser uno de los siguientes: A+, A-, B+, B-, AB+, AB-, O+, O-.");
            return grupo.ToUpper();
        }

        private DateTime ComprobarFechaIngreso(DateTime fecIngreso)
        {

            if (fecIngreso > DateTime.Now) throw new Exception("Error: La fecha de ingreso no puede ser futura");

            return fecIngreso;

        }

        #endregion
    }
}
