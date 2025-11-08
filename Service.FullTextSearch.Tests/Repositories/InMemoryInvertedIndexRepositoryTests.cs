using FluentAssertions;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Domain.Entities;
using Service.FullTextSearch.Infrastructure.Persistence.Repositories;

namespace Service.FullTextSearch.Tests.Repositories;

public class InMemoryInvertedIndexRepositoryTests
{
    private readonly IInvertedIndexRepository _sut;

    public InMemoryInvertedIndexRepositoryTests()
    {
        _sut = new InMemoryInvertedIndexRepository();
    }

    [Fact]
    public void GetByTerm_ShouldReturnIndex_WhenTermExists()
    {
        // Arrange
        var term = "test";
        var index = new InvertedIndex(term);
        _sut.Add(index);

        // Act
        var result = _sut.GetByTerm(term);

        // Assert
        result.Should().BeEquivalentTo(index);
    }

    [Fact]
    public void GetByTerm_ShouldReturnNull_WhenTermDoesNotExist()
    {
        // Arrange
        var term = "nonexistent";

        // Act
        var result = _sut.GetByTerm(term);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetByTerm_ShouldBeCaseInsensitive_WhenTermHasMixedCase()
    {
        // Arrange
        var term = "Test";
        var index = new InvertedIndex(term);
        _sut.Add(index);

        // Act
        var result1 = _sut.GetByTerm("TEST");
        var result2 = _sut.GetByTerm("test");
        var result3 = _sut.GetByTerm("Test");

        // Assert
        result1.Should().BeEquivalentTo(index);
        result2.Should().BeEquivalentTo(index);
        result3.Should().BeEquivalentTo(index);
    }

    [Fact]
    public void GetAll_ShouldReturnAllIndices_WhenIndicesExist()
    {
        // Arrange
        var index1 = new InvertedIndex("term1");
        var index2 = new InvertedIndex("term2");
        var expected = new List<InvertedIndex> { index1, index2 };

        _sut.Add(index1);
        _sut.Add(index2);

        // Act
        var result = _sut.GetAll();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetAll_ShouldReturnEmptyList_WhenNoIndicesExist()
    {
        // Arrange

        // Act
        var result = _sut.GetAll();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Add_ShouldStoreIndex_WhenNewIndexAdded()
    {
        // Arrange
        var term = "newterm";
        var index = new InvertedIndex(term);

        // Act
        var result = _sut.Add(index);

        // Assert
        result.Should().BeEquivalentTo(index);
        _sut.GetByTerm(term).Should().BeEquivalentTo(index);
    }

    [Fact]
    public void Add_ShouldOverwriteExistingIndex_WhenIndexWithSameTermExists()
    {
        // Arrange
        var term = "existingterm";
        var originalIndex = new InvertedIndex(term);
        var updatedIndex = new InvertedIndex(term);

        _sut.Add(originalIndex);

        // Act
        var result = _sut.Add(updatedIndex);

        // Assert
        result.Should().BeEquivalentTo(updatedIndex);
        _sut.GetByTerm(term).Should().BeEquivalentTo(updatedIndex);
    }

    [Fact]
    public void Update_ShouldReplaceExistingIndex_WhenIndexExists()
    {
        // Arrange
        var term = "updateterm";
        var originalIndex = new InvertedIndex(term);
        var updatedIndex = new InvertedIndex(term);

        _sut.Add(originalIndex);

        // Act
        _sut.Update(updatedIndex);

        // Assert
        _sut.GetByTerm(term).Should().BeEquivalentTo(updatedIndex);
    }

    [Fact]
    public void Update_ShouldAddIndex_WhenIndexDoesNotExist()
    {
        // Arrange
        var term = "newupdateterm";
        var index = new InvertedIndex(term);

        // Act
        _sut.Update(index);

        // Assert
        _sut.GetByTerm(term).Should().BeEquivalentTo(index);
    }

    [Fact]
    public void Delete_ShouldRemoveIndex_WhenTermExists()
    {
        // Arrange
        var term = "deleteterm";
        var index = new InvertedIndex(term);
        _sut.Add(index);

        // Act
        _sut.Delete(term);

        // Assert
        _sut.GetByTerm(term).Should().BeNull();
    }

    [Fact]
    public void Delete_ShouldBeCaseInsensitive_WhenDeletingTerm()
    {
        // Arrange
        var term = "DeleteTerm";
        var index = new InvertedIndex(term);
        _sut.Add(index);

        // Act
        _sut.Delete("DELETETERM");

        // Assert
        _sut.GetByTerm(term).Should().BeNull();
    }

    [Fact]
    public void Delete_ShouldDoNothing_WhenTermDoesNotExist()
    {
        // Arrange
        var term = "nonexistent";

        // Act
        _sut.Delete(term);

        // Assert
        _sut.GetByTerm(term).Should().BeNull();
    }

    [Fact]
    public void SearchTerms_ShouldReturnMatchingIndices_WhenSomeTermsExist()
    {
        // Arrange
        var index1 = new InvertedIndex("term1");
        var index2 = new InvertedIndex("term2");
        var index3 = new InvertedIndex("term3");

        _sut.Add(index1);
        _sut.Add(index2);
        _sut.Add(index3);

        var searchTerms = new List<string> { "term1", "term3", "nonexistent" };
        var expected = new List<InvertedIndex> { index1, index3 };

        // Act
        var result = _sut.SearchTerms(searchTerms);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SearchTerms_ShouldReturnEmptyList_WhenNoTermsExist()
    {
        // Arrange
        var searchTerms = new List<string> { "nonexistent1", "nonexistent2" };

        // Act
        var result = _sut.SearchTerms(searchTerms);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void SearchTerms_ShouldBeCaseInsensitive_WhenSearchingTerms()
    {
        // Arrange
        var index = new InvertedIndex("TestTerm");
        _sut.Add(index);

        var searchTerms = new List<string> { "TESTTERM", "testterm", "TestTerm" };

        // Act
        var result = _sut.SearchTerms(searchTerms);

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().BeEquivalentTo(index);
    }

    [Fact]
    public void GetAllIndices_ShouldReturnAllIndicesAsDictionary_WhenIndicesExist()
    {
        // Arrange
        var index1 = new InvertedIndex("term1");
        var index2 = new InvertedIndex("term2");
        var expected = new Dictionary<string, InvertedIndex>
        {
            { "term1", index1 },
            { "term2", index2 }
        };

        _sut.Add(index1);
        _sut.Add(index2);

        // Act
        var result = _sut.GetAllIndices();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetAllIndices_ShouldReturnEmptyDictionary_WhenNoIndicesExist()
    {
        // Arrange

        // Act
        var result = _sut.GetAllIndices();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Operations_ShouldMaintainSeparateIndices_WhenMultipleIndicesExist()
    {
        // Arrange
        var index1 = new InvertedIndex("term1");
        var index2 = new InvertedIndex("term2");
        var index3 = new InvertedIndex("term3");

        _sut.Add(index1);
        _sut.Add(index2);
        _sut.Add(index3);

        // Act
        _sut.Delete("term2");
        var remainingIndices = _sut.GetAll();

        // Assert
        remainingIndices.Should().HaveCount(2);
        remainingIndices.Should().ContainEquivalentOf(index1);
        remainingIndices.Should().ContainEquivalentOf(index3);
        remainingIndices.Should().NotContainEquivalentOf(index2);
    }
    
}