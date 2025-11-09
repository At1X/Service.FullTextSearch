using FluentAssertions;
using NSubstitute;
using Service.FullTextSearch.Application.Common.Interfaces;
using Service.FullTextSearch.Application.DocumentIndexer.Abstraction;
using Service.FullTextSearch.Application.DocumentIndexer.Business;
using Service.FullTextSearch.Application.InvertedIndexDocumentActions.Abstraction;
using Service.FullTextSearch.Domain.Entities;

namespace Service.FullTextSearch.Tests.Services;


public class InvertedIndexServiceTests
{
    private readonly IInvertedIndexRepository _indexRepository;
    private readonly IInvertedIndexDocumentUpdater _invertedIndexDocumentUpdater;
    private readonly IDocumentIndexer _sut;


    public InvertedIndexServiceTests()
    {
        _indexRepository = Substitute.For<IInvertedIndexRepository>();
        _invertedIndexDocumentUpdater = Substitute.For<IInvertedIndexDocumentUpdater>();
        _sut = new InvertedIndexService(_indexRepository, _invertedIndexDocumentUpdater);
    }

    [Fact]
    public void IndexTerms_ShouldAddNewIndex_WhenTermDoesNotExist()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var termFrequencies = new Dictionary<string, int> { { "test", 2 } };
        InvertedIndex capturedIndex = null;
        
        _indexRepository.GetByTerm("test").Returns((InvertedIndex)null);
        _invertedIndexDocumentUpdater.When(x => x.AddOrUpdateDocument(Arg.Is<InvertedIndex>(idx => idx.Term == "test"), documentId, 2))
            .Do(call => capturedIndex = call.Arg<InvertedIndex>());

        // Act
        _sut.IndexTerms(documentId, termFrequencies);

        // Assert
        _indexRepository.Received(1).Add(Arg.Is<InvertedIndex>(idx => idx.Term == "test"));
        _indexRepository.DidNotReceive().Update(Arg.Any<InvertedIndex>());
        capturedIndex.Term.Should().Be("test");
    }

    [Fact]
    public void IndexTerms_ShouldUpdateExistingIndex_WhenTermExists()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var termFrequencies = new Dictionary<string, int> { { "existing", 3 } };
        var existingIndex = new InvertedIndex("existing");
        
        _indexRepository.GetByTerm("existing").Returns(existingIndex);

        // Act
        _sut.IndexTerms(documentId, termFrequencies);

        // Assert
        _invertedIndexDocumentUpdater.Received(1).AddOrUpdateDocument(existingIndex, documentId, 3);
        _indexRepository.Received(1).Update(existingIndex);
        _indexRepository.DidNotReceive().Add(Arg.Any<InvertedIndex>());
    }

    [Fact]
    public void IndexTerms_ShouldProcessMultipleTerms_WhenMultipleTermsProvided()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var termFrequencies = new Dictionary<string, int> 
        { 
            { "term1", 1 },
            { "term2", 2 },
            { "term3", 3 }
        };
        
        _indexRepository.GetByTerm("term1").Returns(new InvertedIndex("term1"));
        _indexRepository.GetByTerm("term2").Returns((InvertedIndex)null);
        _indexRepository.GetByTerm("term3").Returns(new InvertedIndex("term3"));

        // Act
        _sut.IndexTerms(documentId, termFrequencies);

        // Assert
        _invertedIndexDocumentUpdater.Received(1).AddOrUpdateDocument(Arg.Is<InvertedIndex>(idx => idx.Term == "term1"), documentId, 1);
        _invertedIndexDocumentUpdater.Received(1).AddOrUpdateDocument(Arg.Is<InvertedIndex>(idx => idx.Term == "term2"), documentId, 2);
        _invertedIndexDocumentUpdater.Received(1).AddOrUpdateDocument(Arg.Is<InvertedIndex>(idx => idx.Term == "term3"), documentId, 3);
        
        _indexRepository.Received(1).Update(Arg.Is<InvertedIndex>(idx => idx.Term == "term1"));
        _indexRepository.Received(1).Add(Arg.Is<InvertedIndex>(idx => idx.Term == "term2"));
        _indexRepository.Received(1).Update(Arg.Is<InvertedIndex>(idx => idx.Term == "term3"));
    }

    [Fact]
    public void IndexTerms_ShouldCreateNewIndex_WhenGetByTermReturnsNull()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var termFrequencies = new Dictionary<string, int> { { "newterm", 5 } };
        
        _indexRepository.GetByTerm("newterm").Returns((InvertedIndex)null);

        // Act
        _sut.IndexTerms(documentId, termFrequencies);

        // Assert
        _invertedIndexDocumentUpdater.Received(1).AddOrUpdateDocument(Arg.Is<InvertedIndex>(idx => idx.Term == "newterm"), documentId, 5);
        _indexRepository.Received(1).Add(Arg.Is<InvertedIndex>(idx => idx.Term == "newterm"));
        _indexRepository.DidNotReceive().Update(Arg.Any<InvertedIndex>());
    }

    [Fact]
    public void IndexTerms_ShouldUseExistingIndex_WhenGetByTermReturnsIndex()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var termFrequencies = new Dictionary<string, int> { { "existingterm", 4 } };
        var existingIndex = new InvertedIndex("existingterm");
        
        _indexRepository.GetByTerm("existingterm").Returns(existingIndex);

        // Act
        _sut.IndexTerms(documentId, termFrequencies);

        // Assert
        _invertedIndexDocumentUpdater.Received(1).AddOrUpdateDocument(existingIndex, documentId, 4);
        _indexRepository.Received(1).Update(existingIndex);
        _indexRepository.DidNotReceive().Add(Arg.Any<InvertedIndex>());
    }

}