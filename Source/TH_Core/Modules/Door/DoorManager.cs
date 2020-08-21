using System.Collections.Generic;
using THTools.ORM;
using System.Linq;

namespace TH.Core.Modules.Door
{
    using Base.Database;
    using Base.Extensions;
    using Base.Exceptions;
    using Entities;
    using Connection;
    using Glass;
    using Glass.Entities;
    using Wood;
    using Wood.Entities;

    public class DoorManager
    {
        private Repository repository;

        public Repository Repository { get { return this.repository; } }

        public DoorManager()
        {
            this.repository = new Repository();
        }

        ConnectionManager cMan = new ConnectionManager();
        GlassManager gMan = new GlassManager();
        WoodManager wMan = new WoodManager();


        //=== Manage: Door

        /// <summary> Get Door </summary>
        /// <param name="kind">Get DoorKind? Default false.</param>
        /// <param name="connection">Get Connection? Default false.</param>
        /// <param name="glass">Get Glass? Default false.</param>
        public Door GetDoor(int id, bool kind = false, bool connection = false, bool glass = false)
        {
            // Check
            if (id < 1)
                return null;


            Door door = repository.GetEntity<Door>(id);

            if (door == null)
                return null;

            // Get children
            if (kind)
            {
                door.DoorKind = this.GetDoorKind(door.IdDoorKind);
            }

            if (connection)
            {
                door.Connection = cMan.GetConnection(door.IdConnection);
            }

            if (glass) // && door.IdGlass.HasValue)
            {
                MergeGlassIntoDoors(new List<Door>() { door });
                //door.Glass = gMan.GetGlass(door.IdGlass.Value);
            }

            return door;
        }

        /// <summary> Save Door (new or existing). </summary>
        public int SaveDoor(Door door)
        {
            if (door == null)
                throw new CoreException("No Door Specified!");

            // Save Connection
            door.IdConnection = cMan.SaveConnection(door.Connection);

            // Save Door Kind
            door.IdDoorKind = SaveDoorKind(door.DoorKind);

            // Insert or update
            if (door.Id == 0)
            {
                door.Id = repository.Insert(door).InsertId.Value;
            }
            else
            {
                repository.Update(door);
            }

            // Save DoorGlass
            if (!door.DoorGlasses.IsNullOrEmpty())
            {
                door.DoorGlasses.ForEach(x => x.IdDoor = door.Id);
                SaveDoorGlass(door.DoorGlasses);
            }

            // Save DoorWood
            if (!door.DoorWoods.IsNullOrEmpty())
            {
                door.DoorWoods.ForEach(x => x.IdDoor = door.Id);
                SaveDoorWood(door.DoorWoods);
            }

            return door.Id;
        }

        /// <summary> Deletes the Door. </summary>
        public void DeleteDoor(int id)
        {
            // Check
            if (id < 1)
                return;

            Door door = repository.GetEntity<Door>(id);

            if (door == null)
                return;

            // Delete parent
            repository.Delete(door);

            // Delete children
            DeleteDoorKind(door.IdDoorKind);

            DeleteDoorWood(doorIds: new int[] { door.Id });

            DeleteDoorGlass(doorIds: new int[] { door.Id });

            // Delete Connection
            cMan.DeleteConnection(new int[] { door.IdConnection });
        }


        //=== Manage: Door Kind

        /// <summary> Get DoorKind. </summary>
        public DoorKind GetDoorKind(int id)
        {
            // Check
            if (id < 1)
                return null;

            DoorKind kind = repository.GetEntity<DoorKind>(id);

            return kind;
        }

        /// <summary> Get DoorKinds. </summary>
        /// <param name="ids">The Id of the doorKind to retrieve.</param>
        public List<DoorKind> GetDoorKinds(int[] ids)
        {
            if (ids.IsNullOrEmpty())
                return null;

            XQuery q = new XQuery()
                .From<DoorKind>()
                .Where()
                    .Column<DoorKind>(x => x.Id).In(ids);

            List<DoorKind> doorKinds = repository.GetEntities<DoorKind>(q).ToList();

            return doorKinds;
        }

        /// <summary> Save DoorKind (new or existing). </summary>
        public int SaveDoorKind(DoorKind kind)
        {
            if (kind == null)
                throw new CoreException("No DoorKind Specified!");

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

        /// <summary> Deletes the DoorKind. </summary>
        public void DeleteDoorKind(int id)
        {
            // Check
            if (id < 1)
                return;

            XQuery q = new XQuery()
                .Delete()
                .From<DoorKind>()
                .Where()
                    .Column<DoorKind>(x => x.Id).Equals().Value(id);

            repository.Delete(q);
        }


        //=== Manage: Door Wood

        /// <summary> Get Door Wood for the specified doorId. </summary>
        public List<DoorWood> GetDoorWoods(int doorId)
        {
            if (doorId < 1) throw new CoreException("Invalid DoorId specified!");

            XQuery q = new XQuery()
                .From<DoorWood>()
                .Where()
                    .Column<DoorWood>(x => x.IdDoor).Equals().Value(doorId);

            List<DoorWood> doorWoods = repository.GetEntities<DoorWood>(q).ToList();

            return doorWoods;
        }

        /// <summary> Get Door Wood. </summary>
        /// <param name="doorIds">The door ids to retrieve doorWood for.</param>
        /// <param name="woodIds">The wood ids to retrieve doorWood for.</param>
        public List<DoorWood> GetDoorWoods(int[] doorIds = null, int[] woodIds = null)
        {
            if (doorIds.IsNullOrEmpty() && woodIds.IsNullOrEmpty())
                return null;

            XQuery q = new XQuery()
                .From<DoorWood>();

            bool whereSet = false;
            if (!doorIds.IsNullOrEmpty())
            {
                q.Where()
                    .Column<DoorWood>(x => x.IdDoor).In(doorIds);

                whereSet = true;
            }

            if (!woodIds.IsNullOrEmpty())
            {
                if (whereSet)
                    q.And();
                else
                    q.Where();

                q.Column<DoorWood>(x => x.IdWood).In(woodIds);
            }

            List<DoorWood> doorWoods = repository.GetEntities<DoorWood>(q).ToList();

            return doorWoods;
        }


        //=== Manage: Door Glass

        /// <summary> Get Door Glass. </summary>
        /// <param name="doorIds">The door Ids to get the Door Glass for.</param>
        /// <param name="glassIds">The glass Ids to get the Door Glass for.</param>
        public List<DoorGlass> GetDoorGlass(int[] doorIds = null, int[] glassIds = null)
        {
            if (doorIds.IsNullOrEmpty() && glassIds.IsNullOrEmpty())
                return null;

            XQuery q = new XQuery()
                .From<DoorGlass>();

            bool whereSet = false;

            if (!doorIds.IsNullOrEmpty())
            {
                q.Where()
                    .Column<DoorGlass>(x => x.IdDoor).In(doorIds);

                whereSet = true;
            }

            if (!glassIds.IsNullOrEmpty())
            {
                if (whereSet)
                    q.And();
                else
                    q.Where();

                q.Column<DoorGlass>(x => x.IdGlass).In(glassIds);
            }

            List<DoorGlass> doorGlasses = repository.GetEntities<DoorGlass>(q).ToList();

            return doorGlasses;
        }

        /// <summary> Insert new doorGlasses. </summary>
        public void SaveDoorGlass(List<DoorGlass> doorGlasses)
        {
            foreach(DoorGlass glass in doorGlasses)
            {
                glass.IdGlass = gMan.SaveGlass(glass.Glass);
            }

            repository.Insert(doorGlasses);
        }

        /// <summary> Save Door Glass (new or existing). </summary>
        public int SaveDoorGlass(DoorGlass doorGlass)
        {
            if (doorGlass == null)
                throw new CoreException("No DoorGlass Specified!");

            if (doorGlass.Door != null)
            {
                doorGlass.IdDoor = SaveDoor(doorGlass.Door);
            }

            if (doorGlass.Glass != null)
            {
                doorGlass.IdGlass = gMan.SaveGlass(doorGlass.Glass);
            }
            else doorGlass.IdGlass = (int?)null;

            // Insert or update
            if (doorGlass.Id == 0)
            {
                doorGlass.Id = repository.Insert(doorGlass).InsertId.Value;
            }
            else
            {
                repository.Update(doorGlass);
            }

            return doorGlass.Id;
        }

        public void DeleteDoorGlass(int[] ids = null, int[] doorIds = null)
        {
            if (ids.IsNullOrEmpty() && doorIds.IsNullOrEmpty())
                return;

            // Get entities
            XQuery q = new XQuery()
                .Select<DoorGlass>();

            bool whereSet = false;

            if (!ids.IsNullOrEmpty())
            {
                q.Where()
                    .Column<DoorGlass>(x => x.Id).In(ids);
                whereSet = true;
            }

            if (!doorIds.IsNullOrEmpty())
            {
                if (whereSet)
                    q.And();
                else
                    q.Where();

                q.Column<DoorGlass>(x => x.IdDoor).In(doorIds);
            }

            List<DoorGlass> doorGlasses = repository.GetEntities<DoorGlass>(q).ToList();

            // Delete 
            repository.Delete(doorGlasses);

            // Delete childs
            foreach (DoorGlass doorGlass in doorGlasses) if (doorGlass.IdGlass.HasValue)
            {
                gMan.DeleteGlass(doorGlass.IdGlass.Value);
            }
        }


        //=== Manage: Door Wood

        /// <summary> Get Door Wood. </summary>
        /// <param name="doorIds">The door Ids to get the Door Wood for.</param>
        /// <param name="woodIds">The wood Ids to get the Door Wood for.</param>
        /// <returns></returns>
        public List<DoorWood> GetDoorWood(int[] doorIds = null, int[] woodIds = null)
        {
            if (doorIds.IsNullOrEmpty() && woodIds.IsNullOrEmpty())
                return null;

            XQuery q = new XQuery()
                .From<DoorWood>();

            bool whereSet = false;

            if (!doorIds.IsNullOrEmpty())
            {
                q.Where()
                    .Column<DoorWood>(x => x.IdDoor).In(doorIds);

                whereSet = true;
            }

            if (!woodIds.IsNullOrEmpty())
            {
                if (whereSet)
                    q.And();
                else
                    q.Where();

                q.Column<DoorWood>(x => x.IdWood).In(woodIds);
            }

            List<DoorWood> doorWoods = repository.GetEntities<DoorWood>(q).ToList();

            return doorWoods;
        }

        /// <summary> Insert new doorWood.</summary>
        public void SaveDoorWood(List<DoorWood> doorWoods)
        {
            foreach(DoorWood wood in doorWoods)
            {
                wood.IdWood = wMan.SaveWood(wood.Wood);
            }

            repository.Insert(doorWoods);
        }

        /// <summary> Save Door Wood (new or existing). </summary>
        public int SaveDoorWood(DoorWood doorWood)
        {
            if (doorWood == null)
                throw new CoreException("No DoorWood Specified!");

            if (doorWood.Door != null)
            {
                doorWood.IdDoor = SaveDoor(doorWood.Door);
            }

            if (doorWood.Wood != null)
            {
                doorWood.IdWood = wMan.SaveWood(doorWood.Wood);
            }


            // Insert or update
            if (doorWood.Id == 0)
            {
                doorWood.Id = repository.Insert(doorWood).InsertId.Value;
            }
            else
            {
                repository.Update(doorWood);
            }

            return doorWood.Id;
        }

        /// <summary> Deletes doorWood. </summary>
        /// <param name="ids">The id of the doorwood.</param>
        /// <param name="doorIds">The ids of doors in doorwood to be deleted.</param>
        public void DeleteDoorWood(int[] ids = null, int[] doorIds = null)
        {
            if (ids.IsNullOrEmpty() && doorIds.IsNullOrEmpty())
                return;

            // Get entities
            XQuery q = new XQuery()
                .Select<DoorWood>();

            bool whereSet = false;

            if (!ids.IsNullOrEmpty())
            {
                q.Where()
                    .Column<DoorWood>(x => x.Id).In(ids);
                whereSet = true;
            }

            if (!doorIds.IsNullOrEmpty())
            {
                if (whereSet)
                    q.And();
                else
                    q.Where();

                q.Column<DoorWood>(x => x.IdDoor).In(doorIds);
            }

            List<DoorWood> doorWoods = repository.GetEntities<DoorWood>(q).ToList();

            // Delete 
            repository.Delete(doorWoods);

            // Delete childs
            foreach(DoorWood doorWood in doorWoods)
            {
                wMan.DeleteWood(doorWood.IdWood);
            }

        }


        //=== Mergers
        
        /// <summary> Merge Kind into Door. </summary>
        /// <param name="doors">The doors to merge a kind into.</param>
        public void MergeKindIntoDoors(List<Door> doors)
        {
            if (doors.IsNullOrEmpty())
                return;

            // Get
            List<DoorKind> doorKinds = GetDoorKinds(ids: doors.Select(x => x.IdDoorKind).ToArray());
            if (doorKinds.IsNullOrEmpty())
                return;

            foreach (Door door in doors)
            {
                if (!doorKinds.Any(x => x.Id == door.IdDoorKind)) continue;

                door.DoorKind = doorKinds.FirstOrDefault(x => x.Id == door.IdDoorKind);
            }
        }

        /// <summary> Merge glass into doors. </summary>
        /// <param name="doors">The doors to merge glass into.</param>
        public void MergeGlassIntoDoors(List<Door> doors)
        {
            if (doors.IsNullOrEmpty())
                return;

            // Get
            List<DoorGlass> glasses = GetDoorGlass(glassIds: doors.Select(x => x.Id).ToArray());
            if (glasses.IsNullOrEmpty())
                return;

            foreach (Door door in doors)
            {
                if (!glasses.Any(x => x.IdDoor == door.Id)) continue;

                door.DoorGlasses = glasses.Where(x => x.IdDoor == door.Id).ToList();
            }
        }

        /// <summary> Merge Door Wood into doors. </summary>
        /// <param name="doors">The doors to merge wood into.</param>
        public void MergeWoodIntoDoors(List<Door> doors)
        {
            if (doors.IsNullOrEmpty())
                return;

            // Get 
            List<DoorWood> woods = GetDoorWoods(doorIds: doors.Select(x => x.Id).ToArray());
            if (woods.IsNullOrEmpty())
                return;

            foreach(Door door in doors)
            {
                if (!woods.Any(x => x.IdDoor == door.Id)) continue;

                door.DoorWoods = woods.Where(x => x.IdDoor == door.Id).ToList();
            }
            
        }

        /// <summary> Merge glass into DoorGlass. </summary>
        /// <param name="doorGlasses">The DoorGlasses to merge Glass into.</param>
        public void MergeGlassIntoDoorGlass(List<DoorGlass> doorGlasses)
        {
            if (doorGlasses.IsNullOrEmpty())
                return;

            // Get
            List<Glass> glasses = gMan.GetGlass(ids: doorGlasses.Select(x => x.IdGlass.Value).ToArray());

            // Merge
            foreach(DoorGlass doorGlass in doorGlasses)
            {
                if (doorGlass.IdGlass == (int?)null || !glasses.Any(x => x.Id == doorGlass.IdGlass.Value)) continue;

                doorGlass.Glass = glasses.FirstOrDefault(x => x.Id == doorGlass.IdGlass);
            }
        }

        /// <summary> Merge Wood into DoorWood. </summary>
        /// <param name="doorWoods">The doorWoods to merge Wood into.</param>
        public void MergeWoodIntoDoorWood(List<DoorWood> doorWoods)
        {
            if (doorWoods.IsNullOrEmpty())
                return;

            // Get
            List<Wood> woods = wMan.GetWood(ids: doorWoods.Select(x => x.IdWood).ToArray());

            // Merge
            foreach (DoorWood doorWood in doorWoods)
            {
                if (!woods.Any(x => x.Id == doorWood.IdWood)) continue;

                doorWood.Wood = woods.FirstOrDefault(x => x.Id == doorWood.IdWood);
            }
        }
    }
}
