/////////////////////////////////////////////////////////////////////////////////////////////////
// Dashmin
//
// Copyright (c) 2021, AndJon. Todos los derechos reservados.
// Este archivo es confidencial de AndJon. No distribuir.
//
// Developers : Heber Estrada

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Dashmin.Application.Common.Interface;
using Dashmin.Application.Common.Entities;

namespace Dashmin.Infraestructure.Persistence
{
    /// <summary>
    /// Modelo de EF con Identity
    /// </summary>
    public class ApplicationDBContext : DbContext, Dashmin.Application.Common.Interface.IApplicationDBContext
    {
        IConfiguration _configuracion;

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options, IConfiguration configuracion) : base(options)
        {
            _configuracion = configuracion;
        }

        public DbSet<ind_global> ind_global { get; set; }
        public DbSet<ind_ingresos_facturados> ind_ingresos_facturados { get; set; }
        public DbSet<ind_objetivo_cobranza> ind_objetivo_cobranza { get; set; }
        public DbSet<ind_pacientes_egresados> ind_pacientes_egresados { get; set; }
        public DbSet<ind_promedio_diario_urgencias> ind_promedio_diario_urgencias { get; set; }
        public DbSet<ind_promediodiariocirugiasala> ind_promediodiariocirugiasala { get; set; }
        public DbSet<var_antiguedad_saldos> var_antiguedad_saldos { get; set; }
        public DbSet<var_camas_censables> var_camas_censables { get; set; }
        public DbSet<var_cargos_diario> var_cargos_diario { get; set; }
        public DbSet<var_cirugias_paciente> var_cirugias_paciente { get; set; }
        public DbSet<var_cronologico_facturas> var_cronologico_facturas { get; set; }
        public DbSet<var_cuenta_paciente> var_cuenta_paciente { get; set; }
        public DbSet<var_facturacion_tipo_paciente> var_facturacion_tipo_paciente { get; set; }
        public DbSet<var_limite_credito> var_limite_credito { get; set; }
        public DbSet<var_ocupacion_hospitalaria> var_ocupacion_hospitalaria { get; set; }
        public DbSet<var_pacientes_ingresos> var_pacientes_ingresos { get; set; }
        public DbSet<var_paquete_cobranza> var_paquete_cobranza { get; set; }
        public DbSet<var_saldos_bancos> var_saldos_bancos { get; set; }
        public DbSet<var_total_consultas_urgencias> var_total_consultas_urgencias { get; set; }
        public DbSet<var_total_diagnosticos_egreso> var_total_diagnosticos_egreso { get; set; }
        public DbSet<var_total_dias_hospitalizacion> var_total_dias_hospitalizacion { get; set; }
        public DbSet<var_total_egresos> var_total_egresos { get; set; }
        public DbSet<var_total_ingresos_actuales> var_total_ingresos_actuales { get; set; }
        public DbSet<indicador_actualizacion> indicador_actualizacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<ind_global>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuracion.GetConnectionString("PostgresConnectionString"));
        }
    }
}
