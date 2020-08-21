namespace TH.Core.Modules.Wood
{
    using Base.Database;
    using Base.Exceptions;
    using Entities;
    using System.Collections.Generic;
    using System.Linq;
    using Base.Extensions;
    using THTools.ORM;

    /// <summary> The Wood Manager class, contains all logic for the wood module. </summary>
    public class WoodManager
    {
        private Repository repository;

        public Repository Repository { get { return this.repository; } }

        public WoodManager()
        {
            this.repository = new Repository();
        }

        //=== Manage: Wood

        /// <summary> Get Wood. </summary>
        /// <param name="woodKind">Retrieve woodKind? Default true.</param>
        /// <param name="woodGlass">Retrieve woodGlass? Default true.</param>
        /// <param name="woodPaint">Retrieve woodPaint? Default true.</param>
        public Wood GetWood(int id, bool woodKind = true, bool woodGlass = true, bool woodPaint = true)
        {
            // Check
            if (id < 1)
                throw new CoreException("Invalid Id Specified!");

            Wood wood = repository.GetEntity<Wood>(id);

            if (wood == null)
                return null;

            if (woodKind && wood.IdWoodKind.HasValue)
            {
                wood.WoodKind = repository.GetEntity<WoodKind>(wood.IdWoodKind.Value);
            }

            if (woodGlass && wood.IdWoodGlassSlat.HasValue)
            {
                wood.WoodGlassSlat = repository.GetEntity<WoodGlassSlat>(wood.IdWoodGlassSlat.Value);
            }

            if (woodPaint && wood.IdWoodPaintColor.HasValue)
            {
                wood.WoodPaintColor = repository.GetEntity<WoodPaintColor>(wood.IdWoodPaintColor.Value);
            }

            return wood;
        }

        /// <summary> Get multiple wood entities. </summary>
        public List<Wood> GetWood(int[] ids)
        {
            if (ids.IsNullOrEmpty())
                return null;

            XQuery q = new XQuery()
                .From<Wood>()
                .Where()
                    .Column<Wood>(x => x.Id).In(ids);

            return repository.GetEntities<Wood>(q).ToList();
        }

        /// <summary> Saves Wood (new or existing). </summary>
        /// <param name="wood">The wood to save.</param>
        public int SaveWood(Wood wood)
        {
            if (wood == null)
                throw new CoreException("No Wood specified!");

            // Save Wood Kind
            if (wood.WoodKind != null)
            {
                wood.IdWoodKind = SaveKind(wood.WoodKind);
            }

            // Save Wood Glass Slag
            if (wood.WoodGlassSlat != null)
            {
                wood.IdWoodGlassSlat = SaveGlassSlat(wood.WoodGlassSlat);
            }
            // Save Wood paint color
            if (wood.WoodPaintColor != null)
            {
                wood.IdWoodPaintColor = SavePaintColor(wood.WoodPaintColor);
            }

            // Insert or update
            if (wood.Id == 0)
            {
                wood.Id = repository.Insert(wood).InsertId.Value;
            }
            else
            {
                repository.Update(wood);
            }
            return wood.Id;
        }

        /// <summary> Deletes the wood. </summary>
        public void DeleteWood(int id)
        {
            // Check
            if (id < 1)
                return;

            Wood wood = repository.GetEntity<Wood>(id);

            if (wood == null)
                return;

            // Delete parent
            repository.Delete(wood);

            // Delete childs if present
            if (wood.IdWoodKind.HasValue)
            {
                DeleteKind(wood.IdWoodKind.Value);
            }

            if (wood.IdWoodGlassSlat.HasValue)
            {
                DeleteGlassSlat(wood.IdWoodGlassSlat.Value);
            }

            if (wood.IdWoodPaintColor.HasValue)
            {
                DeletePaintColor(wood.IdWoodPaintColor.Value);
            }            
        }

        //=== Manage: Wood Kind

        /// <summary> Get WoodKind. </summary>
        /// <param name="woodPaint">Retrieve woodPaint? Default true.</param>
        public WoodKind GetKind(int id, bool woodPaint = true)
        {
            // Check
            if (id < 1)
                throw new CoreException("Invalid Id Specified!");

            WoodKind woodKind = repository.GetEntity<WoodKind>(id);

            if (woodKind == null)
                return null;

            if (woodPaint && woodKind.IdWoodPaintColor.HasValue)
            {
                woodKind.WoodPaintColor = repository.GetEntity<WoodPaintColor>(woodKind.IdWoodPaintColor.Value);
            }

            return woodKind;
        }

        /// <summary> Saves woodKind (new or existing). </summary>
        /// <param name="woodKind">The woodkind to save.</param>
        public int SaveKind(WoodKind woodKind)
        {
            if (woodKind == null)
                throw new CoreException("No wood kind specified!");

            // Save Wood Paint Color
            if (woodKind.WoodPaintColor != null)
            {
                woodKind.IdWoodPaintColor = SavePaintColor(woodKind.WoodPaintColor);
            }

            // Insert op update
            if (woodKind.Id == 0)
            {
                woodKind.Id = repository.Insert(woodKind).InsertId.Value;
            }
            else
            {
                repository.Update(woodKind);
            }

            return woodKind.Id;
        }

        /// <summary> Deletes the wood kind. </summary>
        /// <param name="id">The id of the wood kind to delete.</param>
        public void DeleteKind(int id)
        {
            if (id < 1)
                return;

            WoodKind kind = repository.GetEntity<WoodKind>(id);

            if (kind == null)
                return;

            // Delete parent
            repository.Delete(kind);

            // Delete childs if present
            if (kind.IdWoodPaintColor.HasValue)
            {
                DeletePaintColor(kind.IdWoodPaintColor.Value);
            }
        }

        //=== Manage: Wood Glass Slat

        /// <summary> Get WoodGlassSlat </summary>
        /// <param name="woodKind">Retrieve woodKind? Default true.</param>
        public WoodGlassSlat GetGlassSlat(int id, bool woodKind = true)
        {
            // Check
            if (id < 1)
                throw new CoreException("Invalid Id Specified!");

            WoodGlassSlat glassSlat = repository.GetEntity<WoodGlassSlat>(id);

            if (glassSlat == null)
                return null;

            if (woodKind && glassSlat.IdWoodKind.HasValue)
            {
                glassSlat.WoodKind = repository.GetEntity<WoodKind>(glassSlat.IdWoodKind.Value);
            }

            return glassSlat;
        }

        /// <summary> Saves WoodGlassSlat (new or existing). </summary>
        /// <param name="glassSlat">The glass Slat to save.</param>
        public int SaveGlassSlat(WoodGlassSlat glassSlat)
        {
            if (glassSlat == null)
                throw new CoreException("No Wood Glass Slat specified!");

            // Save WoodKind
            if (glassSlat.WoodKind != null)
            {
                glassSlat.IdWoodKind = SaveKind(glassSlat.WoodKind);
            }

            // Insert or update
            if (glassSlat.Id == 0)
            {
                glassSlat.Id = repository.Insert(glassSlat).InsertId.Value;
            }
            else
            {
                repository.Update(glassSlat);
            }

            return glassSlat.Id;
        }

        /// <summary> Deletes the wood glass slat. </summary>
        /// <param name="id">The id of the wood glass slat to delete.</param>
        public void DeleteGlassSlat(int id)
        {
            if (id < 1)
                return;

            WoodGlassSlat glassSlat = repository.GetEntity<WoodGlassSlat>(id);

            if (glassSlat == null)
                return;

            // Delete parent
            repository.Delete(glassSlat);

            // Delete childs
            if (glassSlat.IdWoodKind.HasValue)
            {
                DeleteKind(glassSlat.IdWoodKind.Value);
            }
        }
        
        //=== Manage: Wood Paint Color

        /// <summary> Get WoodPaintColor. </summary>
        public WoodPaintColor GetPaintColor(int id)
        {
            // Check
            if (id < 1)
                throw new CoreException("Invalid Id Specified!");

            WoodPaintColor paintColor = repository.GetEntity<WoodPaintColor>(id);

            return paintColor;
        }

        /// <summary> Saves paintColor (new or existing). </summary>
        /// <param name="paintColor">The paint color to save.</param>
        public int SavePaintColor(WoodPaintColor paintColor)
        {
            if (paintColor == null)
                throw new CoreException("No wood paint color specified!");

            // Insert or update
            if (paintColor.Id == 0)
            {
                paintColor.Id = repository.Insert(paintColor).InsertId.Value;
            }
            else
            {
                repository.Update(paintColor);
            }

            return paintColor.Id;
        }

        /// <summary> Deletes the wood paint color. </summary>
        /// <param name="id">The id of the paint color to delete.</param>
        public void DeletePaintColor(int id)
        {
            if (id < 1)
                return;

            XQuery q = new XQuery()
                .Delete()
                .From<WoodPaintColor>()
                .Where()
                    .Column<WoodPaintColor>(x => x.Id).Equals().Value(id);

            repository.Delete(q);
        }
    }
}
