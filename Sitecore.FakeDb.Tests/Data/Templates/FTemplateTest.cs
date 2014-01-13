﻿namespace Sitecore.FakeDb.Tests.Data.Templates
{
  using FluentAssertions;
  using Sitecore.Data;
  using Sitecore.FakeDb.Templates;
  using Xunit;

  public class FTemplateTest
  {
    [Fact]
    public void ShouldGenerateDefaultNameAndId()
    {
      // arrange
      var template = new FTemplate();

      // assert
      template.ID.Should().NotBeNull();
      template.ID.Should().NotBe(ID.Null);

      template.Name.Should().Be(template.ID.ToShortID().ToString());
    }

    [Fact]
    public void ShouldCreateEmptyFieldsCollection()
    {
      // arrange
      var template = new FTemplate();

      // act & assert
      template.Fields.Should().BeEmpty();
    }

    [Fact]
    public void ShouldCreateEmptyFieldsCollectionWhenSetNameAndId()
    {
      // arrange
      var template = new FTemplate("t", ID.NewID);

      // act & assert
      template.Fields.Should().BeEmpty();
    }

    [Fact]
    public void ShouldSetTemplateFields()
    {
      // arrange
      var template = new FTemplate { "Title", "Description" };

      // assert
      template.Fields.ShouldBeEquivalentTo(new[] { "Title", "Description" });
    }
  }
}