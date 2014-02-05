namespace Sitecore.FakeDb
{
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Diagnostics;
  using Sitecore.Data;

  [DebuggerDisplay("Name = {Name}, FullPath = {FullPath}")]
  public class DbItem : IEnumerable
  {
    public DbItem(string name)
      : this(name, ID.NewID)
    {
    }

    public DbItem(string name, ID id)
      : this(name, id, ID.NewID)
    {
    }

    public DbItem(string name, ID id, ID templateId)
    {
      this.Name = name;
      this.ID = id;
      this.TemplateID = templateId;
      this.Fields = new DbFieldCollection();
      this.Children = new Collection<DbItem>();
    }

    public string Name { get; set; }

    public ID ID { get; private set; }

    public ID TemplateID { get; private set; }

    public DbFieldCollection Fields { get; set; }

    public ID ParentID { get; set; }

    public string FullPath { get; internal set; }

    public ICollection<DbItem> Children { get; set; }

    public void Add(string fieldName, string fieldValue)
    {
      this.Fields.Add(fieldName, fieldValue);
    }

    public void Add(DbItem child)
    {
      this.Children.Add(child);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.Children.GetEnumerator();
    }
  }
}