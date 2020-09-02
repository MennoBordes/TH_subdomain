using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TH.Core.Base.Exceptions;

namespace TH.Core.Tools.Form.Models
{
    using FORM = Tools.Form;
    /// <summary> List Item Template. </summary>
    public class ListElementItem
    {
        /// <summary> Template's id. </summary>
        [XmlAttribute]
        public int Id { get; set; }

        /// <summary> The key is used for mapping purposes. </summary>
        [XmlAttribute]
        public string Key { get; set; }


        /// <summary> Template's label. </summary>
        public string Label { get; set; }

        /// <summary> Template's description. </summary>
        public string Description { get; set; }

        /// <summary> Template's status. </summary>
        [XmlAttribute]
        public int Status { get; set; }

        /// <summary> Template's index. </summary>
        [XmlAttribute]
        public int Index { get; set; }

        /// <summary> Display Format. </summary>
        public string DisplayFormat { get; set; }

        /// <summary> Template's thumbnail. </summary>
        public int? ThumbId { get; set; }

        /// <summary> Template's cms source, also used for cms mapping within another lookup. </summary>
        public int? DmSourceId { get; set; }


        /// <summary> Feature: Select. </summary>
        [XmlAttribute]
        public bool Select { get; set; }

        /// <summary> Feature: Delete. </summary>
        [XmlAttribute]
        public bool Delete { get; set; }

        /// <summary> Feature: Edit. </summary>
        [XmlAttribute]
        public bool Edit { get; set; }

        /// <summary> Feature: Copy. </summary>
        [XmlAttribute]
        public bool Copy { get; set; }


        //=== Form

        /// <summary> Blocks. </summary>
        [XmlArray]
        [XmlArrayItem("Block")]
        public List<Block> Blocks { get; set; }

        /// <summary> Metadata. </summary>
        [XmlIgnore]
        public Metadata Metadata { get; set; }

        /// <summary> Adds a field. </summary>
        public void AddField(FormElement formElement, int? blockId = null, int? col = null, int? index = null)
        {
            // Check
            if (formElement == null)
                throw new CoreException("Parameter 'formElement' can not be null.");
            if (formElement.Type == Enums.FormElementType.List)
                throw new CoreException("A list field in another list field is not allowed.");

            // Set id
            if (formElement.Id == 0)
            {
                formElement.Id = this.CreateFieldId();
            }

            // Set column
            if (formElement.Column == 0)
            {
                formElement.Column = (col != null) ? col.Value : 1;
            }

            // Add to Block    
            if (this.Blocks == null)
                this.Blocks = new List<Block>();

            //- Find block
            Block block = this.Blocks.FirstOrDefault(x => x.Id == blockId);
            if (block == null)
            {
                //-- Get default block
                block = this.Blocks.FirstOrDefault(x => x.Id == 0);

                //-- Create default block
                if (block == null)
                {
                    block = new Block();
                    if (blockId != null)
                        block.Id = blockId.Value;
                    this.Blocks.Add(block);
                }
            }

            block.AddFormElement(formElement, index: index);
        }

        /// <summary> Find field by id.</summary>
        public FormElement FindField(int id)
        {
            Column col = this.FindParentCol(x => x.Id == id);

            return col != null ? col.Elements.FirstOrDefault(x => x.Id == id) : null;
        }

        /// <summary> Find field by key.</summary>
        public FormElement FindFieldByKey(string key)
        {
            Column col = this.FindParentCol(x => x.Key == key);

            return col != null ? col.Elements.FirstOrDefault(x => x.Key == key) : null;
        }

        /// <summary> Find field's parent column.</summary>
        public Column FindParentCol(Func<FormElement, bool> predicate)
        {
            if (this.Blocks != null)
            {
                foreach (Block block in this.Blocks)
                {
                    if (block.Columns != null)
                    {
                        foreach (Column col in block.Columns)
                        {
                            if (col.Elements != null)
                            {
                                if (col.Elements.FirstOrDefault(predicate) != null)
                                {
                                    return col;
                                }
                            }
                        }
                    }
                }
            }

            // Not found
            return null;
        }

        /// <summary> Gets all fields. </summary>
        public List<FormElement> GetFields()
        {
            List<FormElement> fields = new List<FormElement>();

            foreach (Block b in Blocks)
            {
                if (b.Columns != null)
                {
                    foreach (Column c in b.Columns)
                    {
                        if (c.Elements != null && c.Elements.Any())
                        {
                            fields.AddRange(c.Elements);
                        }
                    }
                }
            }

            return fields;
        }

        /// <summary> Deletes a field (hard). </summary>
        public void DeleteField(FormElement field)
        {
            Column col = this.FindParentCol(x => x.Id == field.Id);
            if (col != null)
                col.Elements.Remove(field);
        }

        /// <summary> Moves the field. </summary>        
        public bool MoveField(int fieldId, int? prevItemId = null)
        {
            // Get parent col
            Column col = this.FindParentCol(x => x.Id == fieldId);
            if (col == null)
                return false;

            // Get previous field
            if (prevItemId == null)
                prevItemId = 0;
            FormElement prevItem = prevItemId > 0 ? col.Elements.FirstOrDefault(x => x.Id == prevItemId) : null;

            // Get field
            FormElement field = col.Elements.FirstOrDefault(x => x.Id == fieldId);
            if (field == null)
                return false;

            // Update index
            col.Elements.Remove(field);
            if (prevItem == null)
            {
                // Insert at beginning
                col.Elements.Insert(0, field);
            }
            else
            {
                // Insert after previous item
                int prevItemIndex = col.Elements.IndexOf(prevItem);
                col.Elements.Insert(prevItemIndex + 1, field);
            }

            // Reset all indexes
            int index = -1;
            foreach (FormElement fe in col.Elements)
            {
                index++;
                fe.Index = index;
            }

            return true;
        }

        /// <summary> Creates the id for a new field. </summary>        
        public int CreateFieldId()
        {
            // Fetch the highest id from main form
            int highestId = 0;
            if (this.Blocks != null && this.Blocks.Any())
            {
                IEnumerable<Column> _columns = this.Blocks.Where(c => c.Columns != null).SelectMany(c => c.Columns);
                if (_columns.Any())
                {
                    IEnumerable<FormElement> _elements = _columns.Where(e => e.Elements != null).SelectMany(e => e.Elements);
                    if (_elements.Any())
                    {
                        highestId = _elements.Max(f => f.Id);
                    }
                }
            }

            // Set highest id
            highestId++;

            return highestId;
        }

        ///// <summary> Creates a form based on current blocks. </summary>
        //public Form GetForm(SessionContext formContext)
        //{
        //    if (this.Blocks == null || !this.Blocks.Any())
        //        return null;

        //    Form form = new Form();
        //    form.Id = Id;
        //    form.Context = formContext;
        //    if (formContext != null)
        //    {
        //        form.LanguageId = formContext.LanguageId.GetValueOrDefault();
        //    }

        //    if (this.Metadata != null)
        //        form.Metadata = this.Metadata;

        //    this.Blocks.ForEach(x => { form.Blocks.Add(x); });

        //    // Field: Lookup
        //    List<Lookup> lookupFields = form.GetFields(x => x.Type == FORM.Enums.FormElementType.Lookup).Select(x => (Lookup)x).ToList();

        //    foreach (Lookup lookupField in lookupFields)
        //    {
        //        if (string.IsNullOrEmpty(lookupField.SourceUrl))
        //        {
        //            lookupField.SourceUrl = "~/tools/app/lookup";
        //            lookupField.Parameters["source"] = "cms";
        //            lookupField.Parameters["context-portal"] = formContext.PortalId.Value;
        //            lookupField.Parameters["context-account"] = formContext.AccountId.Value;
        //            lookupField.Parameters["context-user"] = formContext.UserId.Value;
        //        }
        //    }

        //    return form;
        //}

        /// <summary> Checks if this item has a usable form. </summary>
        public bool HasForm()
        {
            return (this.Blocks != null && this.Blocks.Any());
        }


        /// <summary> Creates a new instance with the default configuration. </summary>
        public static ListElementItem InitTemplate(int? id = null, string key = null, string label = null, string description = null,
            bool select = true, bool edit = true, bool delete = true, bool copy = false)
        {
            ListElementItem obj = new ListElementItem();

            obj.Id = id ?? 0;
            obj.Key = key;
            obj.Status = (int)Base.Enums.Status.Active;
            obj.Index = 0;

            obj.Label = label;
            obj.Description = description;

            obj.Select = select;
            obj.Edit = edit;
            obj.Delete = delete;
            obj.Copy = copy;

            return obj;
        }
    }
}
