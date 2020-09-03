using System;
using System.Collections.Generic;
using System.Linq;

namespace TH.Core.Tools.Form.Models
{
    using System.Xml.Serialization;
    using TH.Core.Tools.Form.Enums;
    public class Block
    {
        /// <summary> The block's id. </summary>
        [XmlAttribute]
        public int Id { get; set; }

        /// <summary> Indicator if this block is visible. </summary>
        [XmlAttribute]
        public bool Visible { get; set; }

        /// <summary> The column layout. </summary>
        [XmlAttribute]
        public FormColumnLayout FormColumnLayout { get; set; }

        /// <summary> The columns inside this block. 
        /// <para> Obsolete, Refactor: column list is unneccesary. </para>
        /// </summary>
        [XmlArray]
        [XmlArrayItem("Column")]
        public List<Column> Columns { get; set; }

        /// <summary> The block's css class. </summary>
        public string BlockCssClass
        {
            get
            {
                switch (FormColumnLayout)
                {
                    case FormColumnLayout.NoSplit:
                        return "nosplit";
                    case FormColumnLayout.Split2:
                        return "split2";
                    case FormColumnLayout.Split3:
                        return "split3";
                    case FormColumnLayout.Split12:
                        return "split1by2";
                    case FormColumnLayout.Split21:
                        return "split2by1";
                    case FormColumnLayout.Split4:
                        return "split4";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary> The block's Html name. </summary>
        public string Name
        {
            get { return string.Format("form_{0}", Id); }
        }

        /// <summary> Inline css. </summary>
        public string InlineCss
        {
            get { return !Visible ? "style=\"display: none;\"" : null; }
        }

        /// <summary> Collection of Attributes. </summary>
        [XmlIgnore]
        public Dictionary<string, object> Attributes { get; set; }

        [XmlIgnore]
        public string CssClass { get; set; }


        //=== Constructors

        /// <summary> Constructs a new block. </summary>
        public Block()
        {
            Visible = true;

            this.FormColumnLayout = FormColumnLayout.NoSplit;
            this.Columns = new List<Column>();
        }

        /// <summary> Constructs a new block. </summary>
        public Block(FormColumnLayout layout)
        {
            Visible = true;

            this.FormColumnLayout = layout;
            this.Columns = new List<Column>();
        }


        //=== Elements

        /// <summary> Set layout. </summary>
        public void SetLayout(FormColumnLayout layout)
        {
            // 1. Fetch all existing elements into single list
            IList<FormElement> fElements = new List<FormElement>();
            if (Columns != null)
            {
                Columns.ForEach(x =>
                {
                    if (x.Elements != null)
                    {
                        fElements = fElements.Concat(x.Elements).ToList();
                    }
                });
            }

            // 2. Create new column layout for this Block
            this.Visible = true;
            this.FormColumnLayout = layout;
            this.Columns.Clear();        //this.Columns = new List<Column>();
            int columnCount = GetColumnCount();
            for (int i = 0; i < columnCount; i++)
            {
                Columns.Add(new Column { Number = i + 1 });
            }

            // 3. Distribute all elements per column (vertical layout)
            int cElements = fElements.Count;
            int maxElements = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(cElements) / Convert.ToDouble(columnCount)));
            int eIndex = 0;
            for (int i = 1; i <= columnCount; i++)
            {
                Column column = Columns.First(x => x.Number == i);

                for (int j = 1; (j <= maxElements && eIndex < cElements); j++)
                {
                    column.Elements.Add(fElements[eIndex]);

                    eIndex++;
                }
            }
        }

        /// <summary> Gets the column count depend on FormColumnLayout. </summary>
        private int GetColumnCount()
        {
            switch (FormColumnLayout)
            {
                case FormColumnLayout.NoSplit:
                    return 1;
                case FormColumnLayout.Split2:
                case FormColumnLayout.Split12:
                case FormColumnLayout.Split21:
                    return 2;
                case FormColumnLayout.Split3:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Adds the form element to the block and put it into the proper column. </summary>
        public void AddFormElement(FormElement element, int? index = null)
        {
            // Set column
            if (element.Column == 0)
                element.Column = 1;


            // Get column
            Column col = null;
            col = this.Columns.FirstOrDefault(x => x.Number == element.Column);
            if (col == null)
            {
                col = new Column { Number = element.Column };
                this.Columns.Add(col);
            }


            // Set index
            if (element.Index == 0)
            {
                element.Index = (index != null) ? index.Value : col.Elements.Count + 1;
            }


            col.Elements.Add(element);
        }

        /// <summary> Returns all elements in this block. </summary>
        public IEnumerable<FormElement> AllElements()
        {
            return this.Columns.SelectMany(e => e.Elements);
        }

        /// <summary> Returns last element in this block. </summary>
        public FormElement LastElement()
        {
            return this.Columns.SelectMany(e => e.Elements).LastOrDefault();
        }


        //=== Form Building

        /// <summary> Add Input. </summary>
        public Input AddInput(int id)
        {
            return this.AddInput(id, 1);
        }

        /// <summary> Add Input. </summary>
        public Input AddInput(int id, int column)
        {
            FormElement sibling = this.LastElement();

            // Default
            Input element = new Input
            {
                Id = id,
                Index = (sibling != null ? sibling.Index + 1 : 0),
                Visible = true,
                Column = column,
                Required = false
            };

            this.AddFormElement(element);

            return element;
        }

        /// <summary> Add Textarea. </summary>
        public Textarea AddTextarea(int id)
        {
            return this.AddTextarea(id, 1);
        }

        /// <summary> Add Textarea. </summary>
        public Textarea AddTextarea(int id, int column)
        {
            FormElement sibling = this.LastElement();

            // Default
            Textarea element = new Textarea
            {
                Id = id,
                Index = (sibling != null ? sibling.Index + 1 : 0),
                Visible = true,
                Column = column,
                Required = false
            };

            this.AddFormElement(element);

            return element;
        }

        /// <summary> Add Dropdown. </summary>
        public Dropdown AddDropDown(int id)
        {
            return this.AddDropDown(id, 1);
        }

        /// <summary> Add Dropdown. </summary>
        public Dropdown AddDropDown(int id, int column)
        {
            FormElement sibling = this.LastElement();

            // Default
            Dropdown element = new Dropdown
            {
                Id = id,
                Index = (sibling != null ? sibling.Index + 1 : 0),
                Visible = true,
                Column = column,
                Required = false,
                Disabled = false
            };

            this.AddFormElement(element);

            return element;
        }

        /// <summary> Add Radiobutton. </summary>
        public Radiobutton AddRadiobutton(int id)
        {
            return this.AddRadiobutton(id, 1);
        }

        /// <summary> Add Radiobutton. </summary>
        public Radiobutton AddRadiobutton(int id, int column)
        {
            FormElement sibling = this.LastElement();

            // Default
            Radiobutton element = new Radiobutton
            {
                Id = id,
                Index = (sibling != null ? sibling.Index + 1 : 0),
                Visible = true,
                Column = column,
                Required = false,
                Disabled = false
            };

            this.AddFormElement(element);

            return element;
        }
    }
}
