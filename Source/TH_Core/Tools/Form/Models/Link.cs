using System;
using System.Xml.Linq;
using TH.Core.Base.Extensions;
using TH.Core.Tools.Form.Enums;

namespace TH.Core.Tools.Form.Models
{
    public class Link : FormElement
    {
        /// <summary> The actual url. </summary>
        public string Url { get; set; }

        /// <summary> The url's display text. </summary>
        public string DisplayText { get; set; }

        /// <summary> The url's title. </summary>
        public string Title { get; set; }

        /// <summary> The url's target. </summary>
        public LinkTarget Target { get; set; }

        /// <summary> Indicator if the url is value is required. </summary>
        public bool Required { get; set; }

        /// <summary> Enables the url's display text. </summary>
        public bool ShowDisplayText { get; set; }

        /// <summary> Enables the url's title. </summary>
        public bool ShowTitle { get; set; }

        /// <summary> Enables the url's target. </summary>
        public bool ShowTarget { get; set; }

        public Link()
        {
            this.Id = 0;
            this.Type = FormElementType.Link;
            this.Status = (int)Base.Enums.Status.Active;
            this.Visible = true;
            this.Disabled = false;
            this.Index = 0;

            this.Target = LinkTarget.NewWindow;
            this.ShowDisplayText = false;
            this.ShowTitle = false;
            this.ShowTarget = false;
        }

        /// <summary> Retrieves FormElement value. </summary>
        public override object GetElementValue()
        {
            return new LinkData { Url = this.Url, DisplayText = this.DisplayText, Title = this.Title, Target = this.Target };
        }

        /// <summary> Sets FormElement value. </summary>
        public override bool SetElementValue(object value)
        {
            // clear
            if (value == null)
            {
                this.Url = null;
                this.DisplayText = null;
                this.Title = null;
                this.Target = LinkTarget.NewWindow;
                return true;
            }

            // set
            try
            {
                LinkData data = null;

                if (value is LinkData)
                {
                    data = (value as LinkData);
                }
                else if (value is string)
                {
                    data = ConvertXml(value as string);
                }
                else
                {
                    data = new LinkData();
                    data.Url = (string)value;
                }

                if (data != null)
                {
                    this.Url = data.Url;
                    this.DisplayText = data.DisplayText;
                    this.Title = data.Title;
                    this.Target = data.Target ?? LinkTarget.NewWindow; // default
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Sets the required property. </summary>
        public override void SetRequired(bool req)
        {
            this.Required = req;
        }

        /// <summary> Gets the required property. </summary>
        public override bool GetRequired()
        {
            return this.Required;
        }

        /// <summary> Creates a new instance with the default configuration. </summary>
        public static Link Init(int? id = null, string label = null, string description = null, bool? required = null)
        {
            Link obj = new Link();

            if (id != null)
            { obj.Id = id.Value; }

            obj.Label = label;
            obj.Description = description;

            if (required != null)
            { obj.Required = required.Value; }

            return obj;
        }

        /// <summary> Link Target. </summary>
        public enum LinkTarget
        {
            None = 0,
            NewWindow = 1
        }

        /// <summary> Link Data. </summary>
        public class LinkData
        {
            /// <summary> The actual url. </summary>
            public string Url { get; set; }

            /// <summary> The url's display text. </summary>
            public string DisplayText { get; set; }

            /// <summary> The url's title. </summary>
            public string Title { get; set; }

            /// <summary> The url's target. </summary>
            public LinkTarget? Target { get; set; }

            /// <summary> The url. </summary>
            public override string ToString()
            {
                return this.Url;
            }
        }

        #region XML Value conversion

        /// <summary> Returns the data in xml format. </summary>
        public string ValueToXml()
        {
            return ConvertXml((LinkData)this.GetElementValue());
        }

        /// <summary> Parses the xml and sets the data. </summary>
        public void XmlToValue(string xml)
        {
            this.SetElementValue(ConvertXml(xml));
        }

        /// <summary> Converts link data to xml. </summary>
        public static string ConvertXml(LinkData data)
        {
            // Check
            if (data == null)
                return null;

            XDocument doc = new XDocument();
            doc.Add(new XElement("root"));

            doc.Root.Add(new XElement("url",
                    new XAttribute("display", data.DisplayText),
                    new XAttribute("title", data.Title),
                    new XAttribute("target", (data.Target != null ? ((int)data.Target).ToString() : (string)null)), data.Url));

            return doc.Root.ToString();
        }

        /// <summary> Converts xml to link data. </summary>
        public static LinkData ConvertXml(string xml)
        {
            // Check
            if (string.IsNullOrWhiteSpace(xml))
                return null;

            LinkData data = new LinkData();

            if (xml.IsXml())
            {
                XDocument doc = XDocument.Parse(xml);
                XElement xe = (XElement)doc.Root.FirstNode;

                data.Url = xe.Value;
                data.DisplayText = xe.Attribute("display").Value;
                data.Title = xe.Attribute("title").Value;

                if (!string.IsNullOrWhiteSpace(xe.Attribute("target").Value))
                {
                    data.Target = (LinkTarget)Convert.ToInt32(xe.Attribute("target").Value);
                }
            }
            else
            {
                data.Url = xml;
            }

            return data;
        }

        #endregion
    }
}
