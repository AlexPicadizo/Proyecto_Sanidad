using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ModeloBdClases.Clases
{
    public class Usuario
    {
        // Atributos
        private string nombre, apellidos, email, contrasenia;
        private bool isAdmin, isActiva;
        public event PropertyChangedEventHandler PropertyChanged;

        #region RELACIONES
        // Relación de uno a muchos con Pacientes para poder guardar los pacientes asociados
        public List<Paciente> Paciente { get; set; }
        #endregion

        #region CONSTRUCTORES
        /// <summary>
        /// Constructor con los parámetros necesarios para el login
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="contrasenia">Contrasenia del usuario</param>
        public Usuario(string email, string contrasenia)
        {
            Email = email;
            Contrasenia = contrasenia;
        }

        /// <summary>
        /// Constructor con todos los parámetros para agregar a la base de datos
        /// </summary>
        /// <param name="nombre">Nombre del usuario</param>
        /// <param name="apellidos">Apellidos del usuario</param>
        /// <param name="email">Email del usuario</param>
        /// <param name="contrasenia">Contrasenia del usuario</param>
        /// <param name="isAdmin">Es admin o no</param>
        public Usuario(string nombre, string apellidos, string email, string contrasenia, bool isAdmin)
        {
            Nombre = nombre;
            Apellidos = apellidos;
            Email = email;
            Contrasenia = contrasenia;
            IsAdmin = isAdmin;
        }

        /// <summary>
        /// Constructor con el parámetro añadido para activar o desactivar la cuenta
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="apellidos"></param>
        /// <param name="email"></param>
        /// <param name="contrasenia"></param>
        /// <param name="isAdmin"></param>
        /// <param name="isActiva"></param>
        public Usuario(string nombre, string apellidos, string email, string contrasenia, bool isAdmin, bool isActiva)
        {
            Nombre = nombre;
            Apellidos = apellidos;
            Email = email;
            Contrasenia = contrasenia;
            IsAdmin = isAdmin;
            IsActiva = isActiva;
        }
        #endregion

        #region PROPIEDADES
        public string Nombre
        {
            get => nombre;
            set
            {
                nombre = ComprobarNombre(value); // Llamar a la validación
            }
        }

        public string Apellidos
        {
            get => apellidos;
            set
            {
                apellidos = ComprobarApellidos(value); // Llamar a la validación
            }
        }

        public string Email
        {
            get => email;
            set
            {
                email = ComprobarEmail(value); // Llamar a la validación
            }
        }

        public string Contrasenia
        {
            get => contrasenia;
            set
            {
                contrasenia = ComprobarContrasenia(value); // Llamar a la validación
            }
        }

        public bool IsAdmin
        {
            get => isAdmin;
            set => isAdmin = value;
        }

        public bool IsActiva
        {
            get => isActiva;
            set
            {
                if (isActiva != value)
                {
                    isActiva = value;
                    OnPropertyChanged(nameof(IsActiva));
                    OnPropertyChanged(nameof(TextoActivacion));
                    OnPropertyChanged(nameof(ColorActivacion));
                }
            }
        }

        // Propiedad calculada para el texto del label
        public string TextoActivacion => IsActiva ? "Desactivar" : "Activar";

        // Propiedad calculada para el color del switch
        public Color ColorActivacion => IsActiva ? Colors.Green : Colors.Red;
        #endregion

        #region VALIDACIONES
        /// <summary>
        /// Comprueba que el nombre no contiene números ni caracteres especiales
        /// </summary>
        /// <returns>El nombre o los apellidos</returns>
        /// <exception cref="Exception"></exception>
        private string ComprobarNombre(string Nombre)
        {
            if (string.IsNullOrEmpty(Nombre)) throw new Exception("El nombre no puede estar vacío.");

            // Expresión regular que permite letras, espacios, tildes y ñ/Ñ
            Regex regex = new Regex(@"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ ]+$");
            if (!regex.IsMatch(Nombre)) throw new Exception("El nombre no puede contener números ni caracteres especiales.");

            return Nombre;
        }

        /// <summary>
        /// Comprueba que los apellidos no contienen números ni caracteres especiales
        /// </summary>
        /// <param name="Apellido"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private string ComprobarApellidos(string Apellido)
        {
            if (string.IsNullOrEmpty(Apellido)) throw new Exception("Los apellidos no pueden estar vacíos.");

            // Expresión regular que permite letras, espacios, tildes y ñ/Ñ
            Regex regex = new Regex(@"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ ]+$");
            if (!regex.IsMatch(Apellido)) throw new Exception("Los apellidos no pueden contener números ni caracteres especiales.");

            return Apellido;
        }

        /// <summary>
        /// Comprueba que el email es valido
        /// </summary>
        /// <param name="correo">El email</param>
        /// <returns> El email</returns>
        /// <exception cref="Exception"></exception>
        private string ComprobarEmail(string correo)
        {
            if (string.IsNullOrEmpty(correo))
                throw new Exception("El email no puede estar vacío.");

            // Regex simplificado: formato básico + extensiones comunes (.com, .es, .net, etc.)
            Regex regex = new Regex(@"^[\w\.\-]+@[\w\-]+\.(com|es|net|org|edu|gov|mil|biz|info|io|co|uk|de|fr|it)$", RegexOptions.IgnoreCase);

            if (!regex.IsMatch(correo))
                throw new Exception("El email no tiene un formato válido o la extensión no está permitida.");

            return correo;
        }

        /// <summary>
        /// Comprueba que la contraseña tenga al menos 6 caracteres
        /// </summary>
        /// <param name="contrasenia"> La contraseña</param>
        /// <returns> La contraseña</returns>
        /// <exception cref="Exception"></exception>
        public string ComprobarContrasenia(string contrasenia)
        {
            if (string.IsNullOrEmpty(contrasenia)) throw new Exception("La contraseña no puede estar vacía.");
            if (contrasenia.Length < 6) throw new Exception("La contraseña debe tener al menos 6 caracteres.");

            return contrasenia;
        }

        /// <summary>
        /// Metodo para notificar a la UI que una propiedad ha cambiado
        /// </summary>
        /// <param name="nombre"></param>
        protected void OnPropertyChanged(string nombre)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
        }
        #endregion

    }
}
