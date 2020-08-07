using System;
using System.Collections.Generic;
using System.Text;

namespace TH2.Shared.Modules.Glass
{
    using Base.Database;
    using Base.Exceptions;
    using Entities;
    using System.Linq;
    using TH2.Shared.Base.Extensions;
    using THTools.ORM;

    public class GlassManager
    {
        private Repository repository;

        public Repository Repository { get { return this.repository; } }

        public GlassManager()
        {
            this.repository = new Repository();
        }

        //=== Manage: Glass

        /// <summary> Get Glass. </summary>
        /// <param name="id">The id.</param>
        /// <param name="ventilation">Also retrieve ventilation (default true).</param>
        public Glass GetGlass(int id, bool ventilation = true)
        {
            // Check
            if (id < 1)
                throw new CoreException("Invalid Id Specified!");

            Glass glass = repository.GetEntity<Glass>(id);

            if (glass == null)
                return null;

            if (ventilation && glass.IdGlassVentilation.HasValue)
            {
                glass.GlassVentilation = repository.GetEntity<GlassVentilation>(glass.IdGlassVentilation.Value);
            }

            return glass;
        }

        /// <summary> Get multiple glass entities. </summary>
        public List<Glass> GetGlass(int[] ids)
        {
            if (ids.IsNullOrEmpty())
                return null;

            XQuery q = new XQuery()
                .From<Glass>()
                .Where()
                    .Column<Glass>(x => x.Id).In(ids);

            return repository.GetEntities<Glass>(q).ToList();
        }

        /// <summary> Saves Glass (new or existing). </summary>
        public int SaveGlass(Glass glass)
        {
            if (glass == null)
                throw new CoreException("No Glass Specified!");

            // Save ventilation
            if (glass.GlassVentilation != null)
            {
                glass.IdGlassVentilation = SaveVentilation(glass.GlassVentilation);
            }

            // Insert or update
            if (glass.Id == 0)
            {
                glass.Id = repository.Insert(glass).InsertId.Value;
            }
            else
            {
                repository.Update(glass);
            }

            return glass.Id;
        }

        /// <summary> Deletes the Glass. </summary>
        public void DeleteGlass(int id)
        {
            // Check
            if (id < 1)
                return;

            Glass glass = repository.GetEntity<Glass>(id);

            if (glass == null)
                return;

            // Delete parent
            repository.Delete(glass);

            if (glass.IdGlassVentilation.HasValue)
            {
                DeleteVentilation(glass.IdGlassVentilation.Value);
            }
        }

        //=== Manage: Glass Ventilation

        /// <summary> Get Glass Ventilation. </summary>
        public GlassVentilation GetVentilation(int id)
        {
            // Check
            if (id < 1)
                throw new CoreException("Invalid Id Specified!");

            return repository.GetEntity<GlassVentilation>(id);
        }

        /// <summary> Saves Ventilation (new or existing). </summary>
        public int SaveVentilation(GlassVentilation ventilation)
        {
            if (ventilation == null)
                throw new CoreException("No Ventilation Specified!");

            // Insert or update
            if (ventilation.Id == 0)
            {
                ventilation.Id = repository.Insert(ventilation).InsertId.Value;
            }
            else
            {
                repository.Update(ventilation);
            }

            return ventilation.Id;
        }

        /// <summary> Deletes the ventilation </summary>
        public void DeleteVentilation(int id)
        {
            // Check
            if (id < 1)
                return;

            XQuery q = new XQuery()
                .Delete()
                .From<GlassVentilation>()
                .Where()
                    .Column<GlassVentilation>(x => x.Id).Equals().Value(id);

            repository.Delete(q);
        }
    }
}
