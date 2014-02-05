﻿namespace Sitecore.FakeDb.Tests
{
  using FluentAssertions;
  using Xunit;

  public class GettingStarted
  {
    [Fact]
    public void BasicItemManagement()
    {
      // Let's create a fake in-memory database. The code below creates new template 'Home' with default section
      // 'Data' and single field 'Title'. Then it creates new item 'Home' based on the template and sets the 'Title'
      // field value to 'Welcome!':
      using (Db db = new Db { new DbItem("Home") { { "Title", "Welcome!" } } })
      {
        // put your code here.

        // The database is already initialized and test items are created under Sitecore Content item.
        // We can get the home item using db.Database property (FluentAssertions library is used in the examples below):
        Sitecore.Data.Database database = db.Database;
        Sitecore.Data.Items.Item homeItem = database.GetItem("/sitecore/content/home");
        homeItem["Title"].Should().Be("Welcome!");
        homeItem.Fields["Title"].Value.Should().Be("Welcome!");

        // The item can be edited using EditContext:
        using (new Sitecore.Data.Items.EditContext(homeItem))
        {
          homeItem["Title"] = "Hi there!";
        }

        homeItem["Title"].Should().Be("Hi there!");

        // Having the item in place it is easy to add a new child. The code below creates new item 'About' under 
        // the 'Home' item and sets title:
        var templateId = new Sitecore.Data.TemplateID(homeItem.TemplateID);
        var aboutItem = homeItem.Add("About", templateId);
        using (new Sitecore.Data.Items.EditContext(aboutItem))
        {
          aboutItem["Title"] = "About us";
        }

        aboutItem["Title"].Should().Be("About us");

        // Parent and child items can be accessed via appropriate properties:
        homeItem.Children["About"].Paths.FullPath.Should().Be("/sitecore/content/Home/About");
        aboutItem.Parent.Paths.FullPath.Should().Be("/sitecore/content/Home");

        // When the items are no more needed it can be removed:
        homeItem.Delete();
        database.GetItem(homeItem.ID).Should().BeNull();
        database.GetItem(aboutItem.ID).Should().BeNull();
      }
    }

    [Fact]
    public void AdvancedItemHierarchy()
    {
      // The code below shows how to create advanced item hierarchy. First of all it creates Home item and sets 
      // welcome text in the Title field. The Home item contains folder Articles with two children, each of them has got
      // own description specified:
      using (Db db = new Db
        {
          new DbItem("home")
            {
              Fields = new DbFieldCollection { { "Title", "Welcome to Sitecore!" } },
              Children = new[]
                {
                  new DbItem("Articles")
                    {
                      new DbItem("Getting Started") { { "Description", "Articles helping to get started." } },
                      new DbItem("Troubleshooting") { { "Description", "Articles with solutions to common problems." } }
                    }
                }
            }
        })
      {
        Sitecore.Data.Items.Item home = db.GetItem("/sitecore/content/home");
        home["Title"].Should().Be("Welcome to Sitecore!");

        Sitecore.Data.Items.Item articles = db.GetItem("/sitecore/content/home/Articles");
        articles.Should().NotBeNull();

        Sitecore.Data.Items.Item gettingStartedArticle = articles.Children["Getting Started"];
        gettingStartedArticle["Description"].Should().Be("Articles helping to get started.");

        Sitecore.Data.Items.Item troubleshootingArticle = articles.Children["Troubleshooting"];
        troubleshootingArticle["Description"].Should().Be("Articles with solutions to common problems.");
      }
    }

    [Fact]
    public void ShouldSetAsContextItem()
    {
      using (var db = new Db { new DbItem("Home") })
      {
        var item = db.GetItem("/sitecore/content/home");
        using (new Sitecore.Data.Items.ContextItemSwitcher(item))
        {
          Sitecore.Context.Item.Should().Be(item);
        }

        Sitecore.Context.Item.Should().BeNull();
      }
    }
  }
}