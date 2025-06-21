using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloBdClases.BaseDatos
{
    public class apiBD
    {
        // Atributos privados para almacenar la configuración de conexión a la base de datos
        string server;
        string port;
        string baseDatos;
        string usuario;
        string contrasenia;
        string conexion;

        #region CONSTRUCTOR
        /// <summary>
        /// Constructor para la conexión a la base de datos.
        /// Este constructor establece los valores de los parámetros de conexión (servidor, puerto, base de datos, usuario y contraseña).
        /// </summary>
        public apiBD()
        {
           
            // Comentado: Conexión a una base de datos remota
            server = "2131e.h.filess.io";
            port = "61002";
            baseDatos = "sanidadDB_pondbarkam";
            usuario = "sanidadDB_pondbarkam";
            contrasenia = "4b0d273e2b058b955f2d1ad8b02b5b91f989e989";

            // Construcción de la cadena de conexión remota
            conexion = $"Server={server}; Database={baseDatos}; " +
                       $"User Id={usuario}; Password={contrasenia}; Port={port};";

            /*
       // Establezco la conexión a la base de datos local (comentada la opción remota)
       server = "localhost"; // Servidor local
       port = "3306";        // Puerto por defecto para MySQL
       baseDatos = "sanidad_db"; // Nombre de la base de datos local
       usuario = "root";    // Usuario para la base de datos
       contrasenia = "Miguelonguapo7)"; // Contraseña del usuario

       // Construcción de la cadena de conexión usando los parámetros anteriores
       conexion = $"Server={server}; Database={baseDatos}; " +
                  $"User Id={usuario}; Password={contrasenia}; Port={port};";*/
        }
        #endregion

        #region METODOS
        /// <summary>
        /// Método para obtener la conexión a la base de datos.
        /// Crea una nueva instancia de MySqlConnection utilizando la cadena de conexión.
        /// </summary>
        /// <returns>Devuelve una instancia de MySqlConnection</returns>
        public MySqlConnection ConexionBd()
        {
            // Creación de la conexión con la base de datos utilizando la cadena de conexión
            MySqlConnection conexc = new MySqlConnection(conexion);

            // Devuelvo la conexión creada
            return conexc;
        }
        #endregion
    }
}
