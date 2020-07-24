using System;
using System.Collections.Generic;

namespace TH2.Shared.Modules.Connection
{
    using Base.Database;
    using Base.Extensions;
    using Entities;
    using System.Linq;
    using Base.Exceptions;
    using THTools.ORM;

    /// <summary> The Connection Manager class, contains all logic for the connection module </summary>
    public class ConnectionManager
    {
        private Repository repository;
        public Repository Repository { get { return this.repository; } }

        public ConnectionManager()
        {
            this.repository = new Repository();
        }

        //=== Manage: Connection

        /// <summary> Get connection. </summary>
        /// <param name="id">The id of the connection to retrieve.</param>
        /// <param name="corners">Set to 'true' when additional data of corners is required.</param>
        /// <param name="hinges">Set to 'true' when additional data of hinges is required.</param>
        /// <param name="locks">Set to 'true' when additional data of locks is required.</param>
        public Connection GetConnection(int id, bool corners = false, bool hinges = false, bool locks = false)
        {
            // Check
            if (id < 1)
                throw new CoreException("Invalid Id Specified");

            Connection connection = repository.GetEntity<Connection>(id);

            if (connection == null)
                return null;

            if (corners)
            {
                XQuery q = new XQuery()
                    .From<ConnectionCorner>()
                    .Where()
                        .Column<ConnectionCorner>(x => x.IdConnection).Equals().Value(connection.Id);

                connection.Corners = repository.GetEntities<ConnectionCorner>(q).ToList();
            }

            if (hinges)
            {
                XQuery q = new XQuery()
                    .From<ConnectionHinge>()
                    .Where()
                        .Column<ConnectionHinge>(x => x.IdConnection).Equals().Value(connection.Id);

                connection.Hinges = repository.GetEntities<ConnectionHinge>(q).ToList();
            }

            if (locks)
            {
                XQuery q = new XQuery()
                    .From<ConnectionLock>()
                    .Where()
                        .Column<ConnectionLock>(x => x.IdConnection).Equals().Value(connection.Id);

                connection.Locks = repository.GetEntities<ConnectionLock>(q).ToList();
            }

            return connection;
        }

        /// <summary> Save connection (new or existing). </summary>
        /// <param name="conn">The connection to save</param>
        public int SaveConnection(Connection conn)
        {
            // Check
            if (conn == null)
                throw new CoreException("No connection specified");

            // Insert or update
            if (conn.Id == 0)
            {
                conn.Id = repository.Insert(conn).InsertId.Value;

                if (conn.Corners.Count > 0)
                {
                    foreach(var corner in conn.Corners)
                    {
                        corner.IdConnection = conn.Id;
                        //corner.Id = repository.Insert(corner).InsertId.Value;
                    }
                    repository.Insert(conn.Corners);
                }

                if (conn.Hinges.Count > 0)
                {
                    foreach (var hinges in conn.Hinges)
                    {
                        hinges.IdConnection = conn.Id;
                        //hinges.Id = repository.Insert(hinges).InsertId.Value;
                    }
                    repository.Insert(conn.Hinges);
                }

                if (conn.Locks.Count > 0)
                {
                    foreach (var cLocks in conn.Locks)
                    {
                        cLocks.IdConnection = conn.Id;
                        //cLocks.Id = repository.Insert(cLocks).InsertId.Value;
                    }
                    repository.Insert(conn.Locks);
                }
            }
            else
            {
                repository.Update(conn);
            }

            return conn.Id;
        }

        /// <summary> Deletes connections. </summary>
        /// <param name="ids">The connections to delete</param>
        public void DeleteConnection(int[] ids)
        {
            // Check
            if (ids.IsNullOrEmpty())
                return;

            // Delete children
            DeleteChildConnections(ids);

            // Delete parent
            XQuery q = new XQuery()
                .Delete()
                .From<Connection>()
                .Where()
                    .Column<Connection>(x => x.Id).In(ids);

            repository.Delete(q);
        }

        /// <summary> Deletes all childs of the provided connectionIds </summary>
        /// <param name="connectionIds">The connectionIds containing children to delete.</param>
        private void DeleteChildConnections(int[] connectionIds)
        {
            // Check
            if (connectionIds.IsNullOrEmpty())
                return;

            DeleteCornerConnections(connectionIds: connectionIds);
            DeleteHingeConnections(connectionIds: connectionIds);
            DeleteLockConnections(connectionIds: connectionIds);
        }

        //=== Manage: Corners

        /// <summary> Get connectionCorner. </summary>
        /// <param name="id">The id of the corner to get.</param>
        public ConnectionCorner GetConnectionCorner(int id)
        {
            // Check
            if (id < 1)
                throw new CoreException("Invalid Id Specified");

            return repository.GetEntity<ConnectionCorner>(id);
        }

        /// <summary> Get a list of Connection Corners</summary>
        /// <param name="connectionId">The connectionId to retrieve for.</param>
        public List<ConnectionCorner> GetConnectionCorners(int connectionId)
        {
            // Check
            if (connectionId < 1)
                return null;

            // Fetch data
            XQuery q = new XQuery()
                .From<ConnectionCorner>()
                .Where()
                    .Column<ConnectionCorner>(x => x.IdConnection).Equals().Value(connectionId);

            List<ConnectionCorner> corners = repository.GetEntities<ConnectionCorner>(q).ToList();

            return corners;
        }

        /// <summary> Save corner (new or existing). </summary>
        /// <param name="corner">The corner to save.</param>
        public int SaveCornerConnection(ConnectionCorner corner)
        {
            // Check
            if (corner == null)
                throw new CoreException("No corner specified");

            // Insert or update
            if (corner.Id == 0)
            {
                corner.Id = repository.Insert(corner).InsertId.Value;
            }
            else
            {
                repository.Update(corner);
            }

            return corner.Id;
        }

        /// <summary> Deletes corner connections. </summary>
        /// <param name="ids">The ids of the corners to delete.</param>
        /// <param name="connectionIds">The connectionId of the corner to delete.</param>
        public void DeleteCornerConnections(int[] ids = null, int[] connectionIds = null)
        {
            // Check
            if (ids.IsNullOrEmpty() && connectionIds.IsNullOrEmpty())
                return;

            XQuery q = new XQuery()
                .Delete()
                .From<ConnectionCorner>();

            bool whereSet = false;
            if (ids != null)
            {
                q.Where().Column<ConnectionCorner>(x => x.Id).In(ids);
                whereSet = true;
            }

            if (connectionIds != null)
            {
                if (whereSet)
                    q.And();
                else
                    q.Where();

                q.Column<ConnectionCorner>(x => x.IdConnection).In(connectionIds);
            }

            repository.Delete(q);
        }

        //=== Manage: Hinges

        /// <summary> Get connectionHinge. </summary>
        /// <param name="id">The id of the hinge to get.</param>
        public ConnectionHinge GetConnectionHinge(int id)
        {
            // Check
            if (id < 1)
                throw new CoreException("Invalid Id Specified");

            return repository.GetEntity<ConnectionHinge>(id);
        }

        /// <summary> Get a list of Connection Hinges. </summary>
        /// <param name="connectionId">The connectionId to retrieve for.</param>
        public List<ConnectionHinge> GetConnectionHinges(int connectionId)
        {
            // Check
            if (connectionId < 1)
                return null;

            XQuery q = new XQuery()
                .From<ConnectionHinge>()
                .Where()
                    .Column<ConnectionHinge>(x => x.IdConnection).Equals().Value(connectionId);

            List<ConnectionHinge> hinges = repository.GetEntities<ConnectionHinge>(q).ToList();

            return hinges;
        }

        /// <summary> Save hinge (new or existing). </summary>
        /// <param name="hinge">The hinge to save.</param>
        public int SaveHingeConnection(ConnectionHinge hinge)
        {
            // Check
            if (hinge == null)
                throw new CoreException("No hinge specified");

            // Insert or update
            if (hinge.Id == 0)
            {
                hinge.Id = repository.Insert(hinge).InsertId.Value;
            }
            else
            {
                repository.Update(hinge);
            }

            return hinge.Id;
        }

        /// <summary> Deletes hinge connections. </summary>
        /// <param name="ids">The ids of the hinges to delete.</param>
        /// <param name="connectionIds">The connectionId of the hinges to delete.</param>
        public void DeleteHingeConnections(int[] ids = null, int[] connectionIds = null)
        {
            // Check
            if (ids.IsNullOrEmpty() && connectionIds.IsNullOrEmpty())
                return;

            XQuery q = new XQuery()
                .Delete()
                .From<ConnectionHinge>();

            bool whereSet = false;
            if (ids != null)
            {
                q.Where().Column<ConnectionHinge>(x => x.Id).In(ids);
                whereSet = true;
            }

            if (connectionIds != null)
            {
                if (whereSet)
                    q.And();
                else
                    q.Where();

                q.Column<ConnectionHinge>(x => x.IdConnection).In(connectionIds);
            }

            repository.Delete(q);
        }

        //=== Manage: Locks

        /// <summary>Get connectionLock. </summary>
        /// <param name="id">The id of the lock to get.</param>
        public ConnectionLock GetConnectionLock(int id)
        {
            // Check
            if (id < 1)
                throw new CoreException("Invalid Id Specified");

            return repository.GetEntity<ConnectionLock>(id);
        }

        /// <summary> Get a list of Connection Locks. </summary>
        /// <param name="connectionId">The connectionId to retrieve for.</param>
        public List<ConnectionLock> GetConnectionLocks(int connectionId)
        {
            // Check
            if (connectionId < 1)
                return null;

            XQuery q = new XQuery()
                .From<ConnectionLock>()
                .Where()
                    .Column<ConnectionLock>(x => x.IdConnection).Equals().Value(connectionId);

            List<ConnectionLock> locks = repository.GetEntities<ConnectionLock>(q).ToList();

            return locks;
        }

        /// <summary> Deletes lock connections. </summary>
        /// <param name="ids">The ids of the lock to delete.</param>
        /// <param name="connectionIds">The connectionId of the lock to delete.</param>
        public void DeleteLockConnections(int[] ids = null, int[] connectionIds = null)
        {
            // Check
            if (ids.IsNullOrEmpty() && connectionIds.IsNullOrEmpty())
                return;

            XQuery q = new XQuery()
                .Delete()
                .From<ConnectionLock>();

            bool whereSet = false;
            if (ids != null)
            {
                q.Where().Column<ConnectionLock>(x => x.Id).In(ids);
                whereSet = true;
            }

            if (connectionIds != null)
            {
                if (whereSet)
                    q.And();
                else
                    q.Where();

                q.Column<ConnectionLock>(x => x.IdConnection).In(connectionIds);
            }

            repository.Delete(q);
        }

        /// <summary> Save lock (new or existing). </summary>
        /// <param name="cLock">The lock to save.</param>
        public int SaveLockConnection(ConnectionLock cLock)
        {
            // Check
            if (cLock == null)
                throw new CoreException("No lock specified");

            // Insert or update
            if (cLock.Id == 0)
            {
                cLock.Id = repository.Insert(cLock).InsertId.Value;
            }
            else
            {
                repository.Update(cLock);
            }

            return cLock.Id;
        }
    }
}
