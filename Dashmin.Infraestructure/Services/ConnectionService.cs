/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System;
using Npgsql;
using System.Data;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using Dashmin.Application.Common.Interface;

namespace Dashmin.Infraestructure.Services
{
    /// <summary>
    /// Sevicio que implementa la interfaz de <see cref="IConnectionService"/> Su funcion es la de crear conexiones a la base de datos
    /// a partir de la cadena de conexion configurada
    /// <seealso cref="IConnectionService"/>
    /// </summary>
    public class ConnectionService : IConnectionService
    {
        /// <summary>
        /// IConfiguration
        /// </summary>
        IConfiguration _configuration;

        /// <summary>
        /// Constructor cuya función es la de crear una nueva instancia de <see cref="ConnectionService"/>.
        /// </summary>
        /// <param name="configuration"> objeto <see cref="IConfiguration"/> que contiene la conexión default de la base de datos como
        /// uno de sus atributos</param>
        public ConnectionService(IConfiguration configuration)
        {
            _configuration   = configuration;
        }

        /// <summary>
        /// Devuelve una nueva instancia de conexión  de base de datos Oracle
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetOracleDb()
        {
            var conn =  new OracleConnection(_configuration.GetConnectionString("OracleConnectionString"));
            return OpenConnection(conn); 
        }

        /// <summary>
        /// Devuelve una nueva instancia de conexión  de base de datos Oracle
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetNpgsqlDb()
        {
            var conn =  new NpgsqlConnection(_configuration.GetConnectionString("PostgresConnectionString"));
            return OpenConnection(conn); 
        }

        public IDbConnection OpenConnection(IDbConnection conn)  
        {
            try
            {
                if (conn.State == ConnectionState.Closed)  
                {  
                    conn.Open();  
                }  
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return conn; 
        }

        public void CloseConnection(IDbConnection conn)  
        {
            if (conn.State == ConnectionState.Open || conn.State == ConnectionState.Broken)  
            {  
                conn.Close();
            }  
        }
    }
}
