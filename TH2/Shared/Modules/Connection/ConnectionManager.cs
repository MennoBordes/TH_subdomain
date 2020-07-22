using System;
using System.Collections.Generic;

namespace TH2.Shared.Modules.Connection
{
    using Base.Database;
    using Base.Extensions;
    using Entities;
    using System.Linq;
    using Z.EntityFramework.Plus;

    /// <summary> The Connection Manager class, contains all logic for the connection module </summary>
    public class ConnectionManager
    {
        #region Initializer

        private readonly ThDbEntities _context;

        public ConnectionManager(ThDbEntities context)
        {
            _context = context;
        }

        #endregion

        #region Retrievers

        /// <summary> Retrieves the connections for a specified Id. </summary>
        /// <param name="id">The id of the connection to retrieve.</param>
        /// <param name="data">Set to 'true' when additional data of connections is required.</param>
        public Connection GetConnection(int id, bool data = false)
        {
            // Check
            if (id < 1) throw new Exception("Invalid Id Specified");

            Connection connection = _context.Connection.FirstOrDefault(x => x.Id == id);
            
            if (connection == null) return null;

            if (data)
            {
                connection.Corners = this.GetConnectionCorners(connection.Id);
                connection.Hinges = this.GetConnectionHinges(connection.Id);
                connection.Locks = this.GetConnectionLocks(connection.Id);
            }

            return connection;
        }

        /// <summary> Retrieves a list of Connection Corners, for the specified id. </summary>
        /// <param name="connectionId">The ConnectionId to retrieve for.</param>
        public List<ConnectionCorner> GetConnectionCorners(int connectionId)
        {
            // Check
            if (connectionId < 1) return null;

            List<ConnectionCorner> corners = _context.ConnectionCorner
                .Where(x => x.IdConnection == connectionId)
                .ToList();

            if (corners == null) return null;

            return corners;
        }

        /// <summary> Retrieves a list of Connection Hinges, for the specified id. </summary>
        /// <param name="connectionId">The ConnectionId to retrieve for.</param>
        public List<ConnectionHinge> GetConnectionHinges(int connectionId)
        {
            // Check
            if (connectionId < 1) return null;

            List<ConnectionHinge> hinges = _context.ConnectionHinge
                .Where(x => x.IdConnection == connectionId)
                .ToList();

            if (hinges == null) return null;

            return hinges;
        }

        /// <summary> Retrieves a list of Connection Locks, for the specified id. </summary>
        /// <param name="connectionId">The ConnectionId to retrieve for.</param>
        public List<ConnectionLock> GetConnectionLocks(int connectionId)
        {
            // Check
            if (connectionId < 1) return null;

            List<ConnectionLock> locks = _context.ConnectionLock
                .Where(x => x.IdConnection == connectionId)
                .ToList();

            if (locks == null) return null;

            return locks;
        }

        #endregion

        #region Insertion

        /// <summary> Inserts a new connection. </summary>
        /// <param name="connection">The connection to insert</param>
        public int InsertConnection(Connection connection)
        {
            // Check
            if (connection == null) throw new Exception("No connection specified");
            if (connection.Id > 0) throw new Exception("The provided connection already has an ID.");

            // Insert
            _context.Connection.Add(connection);
            //_context.Connection.SingleInsert(connection);
            _context.SaveChanges();

            // Check if corners present
            if (!connection.Corners.IsNullOrEmpty())
            {
                // Set ConnectionId of corner
                connection.Corners.ForEach(x => x.IdConnection = connection.Id);
                BulkInsertConnectionCorners(connection.Corners.ToArray());
            }
            // Check if hinges present
            if (!connection.Hinges.IsNullOrEmpty())
            {
                // Set ConnectionId of hinges
                connection.Hinges.ForEach(x => x.IdConnection = connection.Id);
                BulkInsertConnectionHinges(connection.Hinges.ToArray());
            }
            // Check if locks present
            if (!connection.Locks.IsNullOrEmpty())
            {
                // Set ConnectionId of locks
                connection.Locks.ForEach(x => x.IdConnection = connection.Id);
                BulkInsertConnectionLocks(connection.Locks.ToArray());
            }

            return connection.Id;
        }

        /// <summary> Inserts multiple connectionCorners in a batch. </summary>
        /// <param name="cCorners">The connectionCorners to insert.</param>
        public void BulkInsertConnectionCorners(ConnectionCorner[] cCorners)
        {
            // Insert into DbSet
            //_context.ConnectionCorner.BulkInsert(cCorners);
            _context.ConnectionCorner.AddRange(cCorners);

            // Save changes
            _context.SaveChanges();
        }

        /// <summary> Inserts multiple connectionHinges in a batch. </summary>
        /// <param name="cHinges">The connectionHinges to insert.</param>
        public void BulkInsertConnectionHinges(ConnectionHinge[] cHinges)
        {
            // Insert into DbSet
            //_context.ConnectionHinge.BulkInsert(cHinges);
            _context.ConnectionHinge.AddRange(cHinges);

            // Save changes
            _context.SaveChanges();
        }

        /// <summary> Inserts multiple connectionLocks in a batch. </summary>
        /// <param name="cLocks">The connectionLocks to insert.</param>
        public void BulkInsertConnectionLocks(ConnectionLock[] cLocks)
        {
            // Insert into DbSet
            //_context.ConnectionLock.BulkInsert(cLocks);
            _context.ConnectionLock.AddRange(cLocks);

            // Save changes
            _context.SaveChanges();
        }

        #endregion

        #region Update

        public Connection UpdateConnection(Connection connection)
        {
            if (connection == null || connection.Id < 1)
                throw new Exception("Invalid parameters specified");
            //_context.Connection.SingleUpdate(connection);
            _context.Connection.Update(connection);

            _context.SaveChanges();

            if (!connection.Corners.IsNullOrEmpty())
            {
                UpdateConnectionCorners(corners: connection.Corners.ToArray());
            }

            if (!connection.Hinges.IsNullOrEmpty())
            {
                UpdateConnectionHinges(hinges: connection.Hinges.ToArray());
            }

            if (!connection.Locks.IsNullOrEmpty())
            {
                UpdateConnectionLocks(locks: connection.Locks.ToArray());
            }

            return connection;
        }

        public void UpdateConnectionCorners(ConnectionCorner[] corners)
        {
            _context.ConnectionCorner.UpdateRange(corners);

            _context.SaveChanges();            
        }

        public void UpdateConnectionHinges(ConnectionHinge[] hinges)
        {
            _context.ConnectionHinge.UpdateRange(hinges);

            _context.SaveChanges();
        }

        public void UpdateConnectionLocks(ConnectionLock[] locks)
        {
            _context.ConnectionLock.UpdateRange(locks);

            _context.SaveChanges();
        }

        #endregion

        #region Deletion

        /// <summary> Deletes a connection. </summary>
        /// <param name="connection">The connection to delete.</param>
        public void DeleteConnection(Connection connection)
        {
            // Check
            if (connection == null || connection.Id < 1)
                throw new Exception("Invalid connection specified.");

            // Delete childs if present
            DeleteConnectionChilds(connection.Id);
            
            // Delete connection
            _context.Connection.Where(x => x.Id == connection.Id).Delete();
        }

        /// <summary> Deletes all children of connection. </summary>
        /// <param name="connectionId">The parent connection id.</param>
        private void DeleteConnectionChilds(int connectionId)
        {
            DeleteConnectionCorners(connectionIds: new int[] { connectionId });
            DeleteConnectionHinges(connectionIds: new int[] { connectionId });
            DeleteConnectionLocks(connectionIds: new int[] { connectionId });
        }

        /// <summary> Deletes connection corners. </summary>
        /// <param name="ids">The ids of the corners to delete.</param>
        /// <param name="connectionIds">The ids of the parent. </param>
        public void DeleteConnectionCorners(int[] ids = null, int[] connectionIds = null)
        {
            if (ids.IsNullOrEmpty() && connectionIds.IsNullOrEmpty())
                return;

            // Delete corners with id
            if (!ids.IsNullOrEmpty())
            {
                _context.ConnectionCorner.Where(x => ids.Contains(x.Id)).Delete();
            }

            // Delete corners with connectionId
            if (!connectionIds.IsNullOrEmpty())
            {
                _context.ConnectionCorner.Where(x => connectionIds.Contains(x.IdConnection)).Delete();
            }
        }

        /// <summary> Deletes connection corners. </summary>
        /// <param name="ids">The ids of the corners to delete.</param>
        /// <param name="connectionIds">The ids of the parent. </param>
        public void DeleteConnectionHinges(int[] ids = null, int[] connectionIds = null)
        {
            if (ids.IsNullOrEmpty() && connectionIds.IsNullOrEmpty()) return;

            if (!ids.IsNullOrEmpty())
            {
                // Delete hinges with id
                _context.ConnectionHinge.Where(x => ids.Contains(x.Id)).Delete();
            }

            if (!connectionIds.IsNullOrEmpty())
            {
                // Delete hinges with connectionId
                _context.ConnectionHinge.Where(x => connectionIds.Contains(x.IdConnection)).Delete();
            }
        }

        /// <summary> Deletes connection corners. </summary>
        /// <param name="ids">The ids of the corners to delete.</param>
        /// <param name="connectionIds">The ids of the parent. </param>
        public void DeleteConnectionLocks(int[] ids = null, int[] connectionIds = null)
        {
            if (ids.IsNullOrEmpty() && connectionIds.IsNullOrEmpty())
                return;

            if (!ids.IsNullOrEmpty())
            {
                // Delete locks with id
                _context.ConnectionLock.Where(x => ids.Contains(x.Id)).Delete();
            }

            if (!connectionIds.IsNullOrEmpty())
            {
                // Delete locks with connectionId
                _context.ConnectionLock.Where(x => connectionIds.Contains(x.IdConnection)).Delete();
            }
        }

        #endregion
    }
}
