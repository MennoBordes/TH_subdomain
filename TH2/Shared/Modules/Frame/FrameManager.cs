using System;
using System.Collections.Generic;
using System.Text;

namespace TH2.Shared.Modules.Frame
{
    using Base.Database;
    using Base.Exceptions;
    using Entities;
    using Connection;
    using Glass;
    using Window;
    using Door;
    using THTools.ORM;
    using System.Linq;

    public class FrameManager
    {
        private Repository repository;

        public Repository Repository { get { return this.repository; } }

        public FrameManager()
        {
            this.repository = new Repository();
        }

        ConnectionManager cMan = new ConnectionManager();
        GlassManager gMan = new GlassManager();
        WindowManager wMan = new WindowManager();
        DoorManager dMan = new DoorManager();

        //=== Manage: Frame

        /// <summary> Get Frame. </summary>
        /// <param name="sill">Get Sill? Default false.</param>
        /// <param name="connection">Get Connection? Default false.</param>
        /// <param name="glass">Get Glass? Default false.</param>
        /// <param name="window">Get Window? Default false.</param>
        /// <param name="door">Get Door? Default false.</param>
        /// <returns></returns>
        public Frame GetFrame(int id, bool sill = false, bool connection = false, bool glass = false, bool window = false, bool door = false)
        {
            // Check
            if (id < 1)
                return null;

            Frame frame = repository.GetEntity<Frame>(id);

            if (frame == null)
                return null;

            // Get children
            if (connection)
            {
                frame.Connection = cMan.GetConnection(frame.IdConnection);
            }

            if (sill && frame.IdFrameSill.HasValue)
            {
                frame.FrameSill = GetFrameSill(frame.IdFrameSill.Value);
            }

            if (glass && frame.IdGlass.HasValue)
            {
                frame.Glass = gMan.GetGlass(frame.IdGlass.Value);
            }

            if (window && frame.IdWindow.HasValue)
            {                
                frame.Window = wMan.GetWindow(frame.IdWindow.Value);
            }

            if (door && frame.IdDoor.HasValue)
            {                
                frame.Door = dMan.GetDoor(frame.IdDoor.Value);
            }

            return frame;
        }

        /// <summary> Save Frame (new or existing). </summary>
        public int SaveFrame(Frame frame)
        {
            if (frame == null)
                throw new CoreException("No Frame Specified!");

            // Save connection
            frame.IdConnection = cMan.SaveConnection(frame.Connection);

            // Save Frame Sill
            if (frame.FrameSill != null)
            {
                SaveFrameSill(frame.FrameSill);
            }

            // Save Glass
            if (frame.Glass != null)
            {
                frame.IdGlass = gMan.SaveGlass(frame.Glass);
            }

            // Save Door
            if (frame.Door != null)
            {
                frame.IdDoor = dMan.SaveDoor(frame.Door);
            }

            // Save Window
            if (frame.Window != null)
            {
                frame.IdWindow = wMan.SaveWindow(frame.Window);
            }

            // Insert or update
            if (frame.Id == 0)
            {
                frame.Id = repository.Insert(frame).InsertId.Value;
            }
            else
            {
                repository.Update(frame);
            }

            return frame.Id;
        }

        public void DeleteFrame(int id)
        {
            // Check
            if (id < 1)
                return;

            Frame frame = repository.GetEntity<Frame>(id);

            if (frame == null)
                return;

            // Delete parent
            repository.Delete(frame);

            // Delete Frame Sill
            if (frame.IdFrameSill.HasValue)
            {
                DeleteFrameSill(frame.IdFrameSill.Value);
            }

            // Delete Connection
            cMan.DeleteConnection(new int[] { frame.IdConnection });

            // Delete Glass
            if (frame.IdGlass.HasValue)
            {
                gMan.DeleteGlass(frame.IdGlass.Value);
            }

            // Delete Door
            if (frame.IdDoor.HasValue)
            {
                dMan.DeleteDoor(frame.IdDoor.Value);
            }

            // Delete Window
            if (frame.IdWindow.HasValue)
            {
                wMan.DeleteWindow(frame.IdWindow.Value);
            }
        }

        //=== Manage: Frame Sill

        /// <summary> Get FrameSill. </summary>
        public FrameSill GetFrameSill(int id)
        {
            // Check
            if (id < 1)
                return null;

            FrameSill kind = repository.GetEntity<FrameSill>(id);

            return kind;
        }

        /// <summary> Save FrameSill (new or existing). </summary>
        public int SaveFrameSill(FrameSill frameSill)
        {
            if (frameSill == null)
                throw new CoreException("No ~FrameSill Specified");

            // Insert or update
            if (frameSill.Id == 0)
            {
                frameSill.Id = repository.Insert(frameSill).InsertId.Value;
            }
            else
            {
                repository.Update(frameSill);
            }

            return frameSill.Id;
        }

        /// <summary> Deletes FrameSill. </summary>
        public void DeleteFrameSill(int id)
        {
            // Check
            if (id < 1)
                return;

            XQuery q = new XQuery()
                .Delete()
                .From<FrameSill>()
                .Where()
                    .Column<FrameSill>(x => x.Id).Equals().Value(id);

            repository.Delete(q);
        }

        //=== Manage: Door Wood

        /// <summary> Get Door Wood. </summary>
        public List<FrameWood> GetFrameWood(int idFrame)
        {
            if (idFrame < 1)
                throw new CoreException("Invalid FrameId specified!");

            XQuery q = new XQuery()
                .From<FrameWood>()
                .Where()
                    .Column<FrameWood>(x => x.IdFrame).Equals().Value(idFrame);

            List<FrameWood> frameWoods = repository.GetEntities<FrameWood>(q).ToList();

            return frameWoods;
        }
    }
}
