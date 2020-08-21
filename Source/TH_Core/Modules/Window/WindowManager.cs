using System.Collections.Generic;
using System.Linq;

namespace TH.Core.Modules.Window
{
    using Base.Database;
    using Base.Exceptions;
    using Entities;
    using Glass;
    using Connection;
    using THTools.ORM;

    public class WindowManager
    {
        private Repository repository;

        public Repository Repository { get { return this.repository; } }

        public WindowManager()
        {
            this.repository = new Repository();
        }

        ConnectionManager cMan = new ConnectionManager();
        GlassManager gMan = new GlassManager();

        //=== Manage: Window

        /// <summary> Get Window. </summary>
        /// <param name="kind">Get WindowKind? Default false.</param>
        /// <param name="connection">>Get Connection? Default false.</param>
        /// <param name="glass">Get Glass? Default false.</param>
        /// <returns></returns>
        public Window GetWindow(int id, bool kind = false, bool connection = false, bool glass = false)
        {
            // Check
            if (id < 1)
                return null;


            Window window = repository.GetEntity<Window>(id);

            if (window == null)
                return null;

            // Get children
            if (kind)
            {
                window.WindowKind = GetWindowKind(window.IdWindowKind);
            }

            if (connection)
            {
                window.Connection = cMan.GetConnection(window.IdConnection);
            }

            if (glass && window.IdGlass.HasValue)
            {
                window.Glass = gMan.GetGlass(window.IdGlass.Value);
            }

            return window;
        }

        /// <summary> Save WIndow (new or existing). </summary>
        public int SaveWindow(Window window)
        {
            if (window == null)
                throw new CoreException("No Window Specified!");

            // Save connection
            window.IdConnection = cMan.SaveConnection(window.Connection);

            // Save WindowKind
            window.IdWindowKind = SaveWindowKind(window.WindowKind);

            // Save Glass
            if (window.Glass != null)
            {
                window.IdGlass = gMan.SaveGlass(window.Glass);
            }

            // Insert or update
            if (window.Id == 0)
            {
                window.Id = repository.Insert(window).InsertId.Value;
            }
            else
            {
                repository.Update(window);
            }

            return window.Id;
        }

        /// <summary> Deletes the Window. </summary>
        public void DeleteWindow(int id)
        {
            if (id < 1)
                return;

            Window window = repository.GetEntity<Window>(id);

            if (window == null)
                return;

            // Delete parent
            repository.Delete(window);

            // Delete children
            DeleteWindowKind(window.IdWindowKind);

            // Delete Connenction
            cMan.DeleteConnection(new int[] { window.IdConnection });

            // Delete Glass
            if (window.IdGlass.HasValue)
            {
                gMan.DeleteGlass(window.IdGlass.Value);
            }

        }

        //=== Manage: Window Kind

        /// <summary> Get Window Kind. </summary>
        public WindowKind GetWindowKind(int id)
        {
            // Check
            if (id < 1)
                return null;

            WindowKind kind = repository.GetEntity<WindowKind>(id);

            return kind;
        }

        /// <summary> Save WindowKind (new or existing). </summary>
        public int SaveWindowKind(WindowKind kind)
        {
            if (kind == null)
                throw new CoreException("No WindowKind Specified!");

            // Insert or update
            if (kind.Id == 0)
            {
                kind.Id = repository.Insert(kind).InsertId.Value;
            }
            else
            {
                repository.Update(kind);
            }

            return kind.Id;
        }

        /// <summary> Deletes the WindowKind. </summary>
        public void DeleteWindowKind(int id)
        {
            // Check
            if (id < 1)
                return;

            XQuery q = new XQuery()
                .Delete()
                .From<WindowKind>()
                .Where()
                    .Column<WindowKind>(x => x.Id).Equals().Value(id);

            repository.Delete(q);
        }

        //=== Manage: Door Wood

        /// <summary> Get Window Wood. </summary>
        public List<WindowWood> GetWindowWood(int idWindow)
        {
            if (idWindow < 1)
                throw new CoreException("Invalid WindowId specified!");

            XQuery q = new XQuery()
                .From<WindowWood>()
                .Where()
                    .Column<WindowWood>(x => x.IdWindow).Equals().Value(idWindow);

            List<WindowWood> windowWoods = repository.GetEntities<WindowWood>(q).ToList();

            return windowWoods;
        }
    }
}
