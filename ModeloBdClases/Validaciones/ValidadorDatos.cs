using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModeloBdClases.Validaciones
{
    public static class ValidadorDatos
    {
        #region USUARIO
        public static string ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre no puede estar vacío.");

            if (nombre.Length < 2)
                throw new Exception("El nombre debe tener al menos 2 caracteres.");

            if (!Regex.IsMatch(nombre, @"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ ]+$"))
                throw new Exception("El nombre solo puede contener letras y espacios.");

            return nombre;
        }

        public static string ValidarApellidos(string apellidos)
        {
            if (string.IsNullOrWhiteSpace(apellidos))
                throw new Exception("Los apellidos no pueden estar vacíos.");

            if (!Regex.IsMatch(apellidos, @"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ ]+$"))
                throw new Exception("Los apellidos solo pueden contener letras y espacios.");

            return apellidos;
        }

        public static string ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("El correo no puede estar vacío.");

            if (!email.Contains("@"))
                throw new Exception("El correo debe contener el símbolo '@'.");

            if (!email.Contains("."))
                throw new Exception("El correo debe contener un dominio como '.com', '.es', etc.");

            Regex regex = new Regex(@"^[\w\.\-]+@[\w\-]+\.[a-zA-Z]{2,}$", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(email))
                throw new Exception("El formato del correo no es válido. Ejemplo: usuario@dominio.com");

            return email;
        }

        public static string ValidarContrasenia(string contrasenia)
        {
            if (string.IsNullOrWhiteSpace(contrasenia))
                throw new Exception("La contraseña no puede estar vacía.");

            if (contrasenia.Length < 6)
                throw new Exception("La contraseña debe tener al menos 6 caracteres.");

            if (!Regex.IsMatch(contrasenia, @"[A-Z]"))
                throw new Exception("La contraseña debe contener al menos una letra mayúscula.");

            if (!Regex.IsMatch(contrasenia, @"[0-9]"))
                throw new Exception("La contraseña debe contener al menos un número.");

            return contrasenia;
        }
        #endregion 

        #region UBICACIÓN
        /// <summary>
        /// Valida que la planta sea un valor positivo.
        /// </summary>
        public static int ComprobarPlanta(int planta)
        {
            if (planta <= 0) throw new Exception("La planta debe ser un número mayor a 0.");
            if (planta > 26) throw new Exception("La planta debe ser un número menor a 26.");
            return planta;
        }

        /// <summary>
        /// Valida que la habitación sea un número positivo.
        /// </summary>
        public static int ComprobarHabitacion(int habitacion)
        {
            if (habitacion <= 0) throw new Exception("La habitación debe ser un número mayor a 0.");
            return habitacion;
        }

        /// <summary>
        /// Valida que la cama sea un número positivo.
        /// </summary>
        public static int ComprobarCama(int cama)
        {
            if (cama <= 0) throw new Exception("La cama debe ser un número mayor a 0.");
            if (cama > 6) throw new Exception("La cama debe ser un número menor a 6.");
            return cama;
        }
        #endregion

        #region PACIENTE
        /// <summary>
        /// Valida que el nombre contenga solo letras, tildes y espacios.
        /// </summary>
        public static string ComprobarNombre(string Nombre)
        {
            if (string.IsNullOrEmpty(Nombre)) throw new Exception("El nombre no puede estar vacío.");
            Regex regex = new Regex(@"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ ]+$");
            if (!regex.IsMatch(Nombre)) throw new Exception("El nombre no puede contener números ni caracteres especiales.");
            return Nombre;
        }

        /// <summary>
        /// Valida que los apellidos contengan solo letras, tildes y espacios.
        /// </summary>
        public static string ComprobarApellidos(string Apellido)
        {
            if (string.IsNullOrEmpty(Apellido)) throw new Exception("Los apellidos no pueden estar vacíos.");
            Regex regex = new Regex(@"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ ]+$");
            if (!regex.IsMatch(Apellido)) throw new Exception("Los apellidos no pueden contener números ni caracteres especiales.");
            return Apellido;
        }

        /// <summary>
        /// Valida que la edad sea un valor mayor a cero.
        /// </summary>
        public static int ComprobarEdad(int edad)
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
        public static string ComprobarDni(string dni)
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
        public static string ComprobarGrupoSanguineo(string grupo)
        {

            if (string.IsNullOrEmpty(grupo))
                throw new Exception("Debe seleccionar un grupo sanguíneo.");

            string[] gruposValidos = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            if (Array.IndexOf(gruposValidos, grupo.ToUpper()) == -1)
                throw new Exception("El grupo sanguíneo debe ser uno de los siguientes: A+, A-, B+, B-, AB+, AB-, O+, O-.");
            return grupo.ToUpper();
        }

        public static DateTime ComprobarFechaIngreso(DateTime fecIngreso)
        {

            if (fecIngreso > DateTime.Now) throw new Exception("Error: La fecha de ingreso no puede ser futura");

            return fecIngreso;

        }

        #endregion

        #region NOTAS EVOLUCIÓN
        public static string ComprobarContenido(string contenido)
        {
            if (string.IsNullOrEmpty(contenido))
                throw new Exception("El contenido de la nota no puede estar vacío.");
            return contenido;
        }

        public static DateTime ValidarFecha(DateTime fecha)
        {
            if (fecha > DateTime.Now)
                throw new Exception("La fecha de la nota no puede ser futura.");
            if (fecha.Equals(null)) throw new Exception("La fecha de la nota no puede estar vacía.");
            return fecha;
        }
        #endregion

        #region MEDICAMENTOS
        /// <summary>
        /// Valida que el nombre del medicamento no sea vacío ni nulo.
        /// </summary>
        /// <param name="nombre">Nombre a validar</param>
        /// <returns>El nombre si es válido</returns>
        /// <exception cref="Exception">Si el nombre está vacío</exception>
        public static string ComprobarNombreMedicamento(string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) throw new Exception("El nombre del medicamento no puede estar vacío.");
            return nombre;
        }

        /// <summary>
        /// Valida que la descripción del medicamento no sea vacía ni nula.
        /// </summary>
        /// <param name="descripcion">Descripción a validar</param>
        /// <returns>La descripción si es válida</returns>
        /// <exception cref="Exception">Si la descripción está vacía</exception>
        public static string ComprobarDescripcionMedicamento(string descripcion)
        {
            if (string.IsNullOrEmpty(descripcion)) throw new Exception("La descripción del medicamento no puede estar vacía.");
            return descripcion;
        }
        #endregion

        #region DIAGNOSTICO

        /// <summary>
        /// Verifica que la fecha del diagnóstico no sea futura.
        /// </summary>
        public static DateTime ComprobarFechaDiagnostico(DateTime fechaDiagnostico)
        {
            if (fechaDiagnostico > DateTime.Now)
                throw new Exception("La fecha del diagnóstico no puede ser futura.");
            return fechaDiagnostico;
        }

        /// <summary>
        /// Verifica que el nombre no esté vacío.
        /// </summary>
        public static string ComprobarNombreDiagnostico(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
                throw new Exception("El nombre del diagnóstico no puede estar vacío.");
            return nombre;
        }

        /// <summary>
        /// Verifica que la descripción no esté vacía.
        /// </summary>
        public static string ComprobarDescripcionDiagnostico(string descripcion)
        {
            if (string.IsNullOrEmpty(descripcion))
                throw new Exception("La descripción del diagnóstico no puede estar vacía.");
            return descripcion;
        }

        #endregion

        #region ALERGIAS

        /// <summary>
        /// Verifica que el nombre de la alergia no esté vacío ni nulo.
        /// </summary>
        /// <param name="nombre">El nombre de la alergia</param>
        /// <returns>El nombre de la alergia si es válido</returns>
        /// <exception cref="Exception">Si el nombre está vacío o nulo</exception>
        public static string ComprobarNombreAlergia(string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
                throw new Exception("El nombre de la alergia no puede estar vacío.");
            return nombre;
        }

        #endregion
    }
}
